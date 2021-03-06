﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Windows;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.IconPacks;
using Microsoft.Extensions.Logging;
using Stylet;

namespace Lithnet.AccessManager.Server.UI
{
    public class LapsConfigurationViewModel : Screen
    {
        private readonly ICertificateProvider certificateProvider;

        private readonly IDirectory directory;

        private readonly IX509Certificate2ViewModelFactory certificate2ViewModelFactory;

        private readonly IDialogCoordinator dialogCoordinator;

        private readonly IServiceSettingsProvider serviceSettings;

        private readonly ILogger<LapsConfigurationViewModel> logger;

        public LapsConfigurationViewModel(IDialogCoordinator dialogCoordinator, ICertificateProvider certificateProvider, IDirectory directory, IX509Certificate2ViewModelFactory certificate2ViewModelFactory, IServiceSettingsProvider serviceSettings, ILogger<LapsConfigurationViewModel> logger)
        {
            this.directory = directory;
            this.certificateProvider = certificateProvider;
            this.certificate2ViewModelFactory = certificate2ViewModelFactory;
            this.dialogCoordinator = dialogCoordinator;
            this.serviceSettings = serviceSettings;
            this.logger = logger;
            this.Forests = new List<Forest>();
            this.AvailableCertificates = new BindableCollection<X509Certificate2ViewModel>();
            this.DisplayName = "Local admin passwords";
        }

        protected override void OnInitialActivate()
        {
            Task.Run(async () =>
            {
                this.BuildForests();
                this.SelectedForest = this.Forests.FirstOrDefault();
                await this.RefreshAvailableCertificates();
            });
        }

        private void BuildForests()
        {
            try
            {
                var domain = Domain.GetCurrentDomain();
                this.Forests.Add(domain.Forest);

                foreach (var trust in domain.Forest.GetAllTrustRelationships().OfType<TrustRelationshipInformation>())
                {
                    if (trust.TrustDirection == TrustDirection.Inbound ||
                        trust.TrustDirection == TrustDirection.Bidirectional)
                    {
                        var forest =
                            Forest.GetForest(new DirectoryContext(DirectoryContextType.Forest, trust.TargetName));
                        this.Forests.Add(forest);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(EventIDs.UIGenericError, ex, "Could not build forest list");
                this.dialogCoordinator.ShowMessageAsync(this, "Error", $"Could not build the forest list\r\n{ex.Message}").ConfigureAwait(false).GetAwaiter().GetResult();
            }
        }

        public List<Forest> Forests { get; }

        public Forest SelectedForest { get; set; }

        private void OnSelectedForestChanged()
        {
            _ = this.RefreshAvailableCertificates();
        }

        public X509Certificate2ViewModel SelectedCertificate { get; set; }

        [PropertyChanged.DependsOn(nameof(SelectedForest))]
        public BindableCollection<X509Certificate2ViewModel> AvailableCertificates { get; }

        public bool CanPublishSelectedCertificate => !this.SelectedCertificate?.IsPublished ?? false;

        public void PublishSelectedCertificate()
        {
            var de = this.directory.GetConfigurationNamingContext(this.SelectedForest.RootDomain.Name);
            var certData = Convert.ToBase64String(this.SelectedCertificate.Model.RawData, Base64FormattingOptions.InsertLineBreaks);

            var vm = new ScriptContentViewModel(this.dialogCoordinator)
            {
                HelpText = "Run the following script to publish the encryption certificate",
                ScriptText = ScriptTemplates.PublishCertificateTemplate
                    .Replace("{configurationNamingContext}", de.GetPropertyString("distinguishedName"))
                    .Replace("{certificateData}", certData)
                    .Replace("{forest}", this.SelectedForest.Name)
            };

            ExternalDialogWindow w = new ExternalDialogWindow
            {
                DataContext = vm,
                SaveButtonVisible = false,
                CancelButtonName = "Close"
            };

            w.ShowDialog();

            try
            {
                if (this.certificateProvider.TryGetCertificateFromDirectory(out X509Certificate2 publishedCert,
                    this.SelectedForest.RootDomain.Name))
                {
                    if (publishedCert.Thumbprint == this.SelectedCertificate.Model.Thumbprint)
                    {
                        this.SelectedCertificate.IsPublished = true;

                        foreach (var c in this.AvailableCertificates.ToList())
                        {
                            if (this.SelectedCertificate != c)
                            {
                                c.IsPublished = false;
                            }

                            if (c.IsOrphaned)
                            {
                                this.AvailableCertificates.Remove(c);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogWarning(EventIDs.UIGenericWarning, ex, "Could not update certificate publication information");
            }
        }

        public bool CanGenerateEncryptionCertificate { get; set; } = true;

        public async Task GenerateEncryptionCertificate()
        {
            try
            {
                X509Certificate2 cert = this.certificateProvider.CreateSelfSignedCert(this.SelectedForest.Name);

                using X509Store store =
                    this.certificateProvider.OpenServiceStore(Lithnet.AccessManager.Constants.ServiceName,
                        OpenFlags.ReadWrite);
                store.Add(cert);

                var vm = this.certificate2ViewModelFactory.CreateViewModel(cert);

                this.AvailableCertificates.Add(vm);
                this.SelectedCertificate = vm;
            }
            catch (Exception ex)
            {
                logger.LogError(EventIDs.UIGenericError, ex, "Could not generate encryption certificate");
                await this.dialogCoordinator.ShowMessageAsync(this, "Error", $"Could not generate the certificate\r\n{ex.Message}");
            }
        }

        public bool CanShowCertificateDialog => this.SelectedCertificate != null;

        public void ShowCertificateDialog()
        {
            X509Certificate2UI.DisplayCertificate(this.SelectedCertificate.Model, this.GetHandle());
        }

        public void DelegateServicePermission()
        {
            var vm = new ScriptContentViewModel(this.dialogCoordinator)
            {
                HelpText = "Modify the OU variable in this script, and run it with domain admin rights to assign permissions for the service account to be able to read the encrypted local admin passwords and history from the directory",
                ScriptText = ScriptTemplates.GrantAccessManagerPermissions.Replace("{serviceAccount}", this.serviceSettings.GetServiceAccount().ToString(), StringComparison.OrdinalIgnoreCase)
            };

            ExternalDialogWindow w = new ExternalDialogWindow
            {
                DataContext = vm,
                SaveButtonVisible = false,
                CancelButtonName = "Close"
            };

            w.ShowDialog();
        }

        public async Task OpenAccessManagerAgentDownload()
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = Constants.LinkDownloadAccessManagerAgent,
                    UseShellExecute = true
                };

                Process.Start(psi);
            }
            catch (Exception ex)
            {
                logger.LogWarning(EventIDs.UIGenericWarning, ex, "Could not open link");
                await this.dialogCoordinator.ShowMessageAsync(this, "Error", $"Could not open the default link handler\r\n{ex.Message}");
            }
        }

        public async Task OpenMsLapsDownload()
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = Constants.LinkDownloadMsLaps,
                    UseShellExecute = true
                };

                Process.Start(psi);
            }
            catch (Exception ex)
            {
                logger.LogWarning(EventIDs.UIGenericWarning, ex, "Could not open link");
                await this.dialogCoordinator.ShowMessageAsync(this, "Error", $"Could not open the default link handler\r\n{ex.Message}");
            }
        }

        public void DelegateMsLapsPermission()
        {
            var vm = new ScriptContentViewModel(this.dialogCoordinator)
            {
                HelpText = "Modify the OU variable in this script, and run it with domain admin rights to assign permissions for the service account to be able to read Microsoft LAPS passwords from the directory",
                ScriptText = ScriptTemplates.GrantMsLapsPermissions.Replace("{serviceAccount}", this.serviceSettings.GetServiceAccount().ToString(), StringComparison.OrdinalIgnoreCase)
            };

            ExternalDialogWindow w = new ExternalDialogWindow
            {
                DataContext = vm,
                SaveButtonVisible = false,
                CancelButtonName = "Close"
            };

            w.ShowDialog();
        }

        private async Task RefreshAvailableCertificates()
        {
            try
            {
                if (this.AvailableCertificates == null)
                {
                    return;
                }

                this.AvailableCertificates.Clear();

                if (this.SelectedForest == null)
                {
                    return;
                }

                var allCertificates = certificateProvider.GetEligibleCertificates(false).OfType<X509Certificate2>();
                this.certificateProvider.TryGetCertificateFromDirectory(out X509Certificate2 publishedCert,
                    this.SelectedForest.RootDomain.Name);

                bool foundPublished = false;

                foreach (var certificate in allCertificates)
                {
                    var vm = this.certificate2ViewModelFactory.CreateViewModel(certificate);

                    if (certificate.Thumbprint == publishedCert?.Thumbprint)
                    {
                        vm.IsPublished = true;
                        foundPublished = true;
                    }

                    if (certificate.Subject.StartsWith($"CN={this.SelectedForest.RootDomain.Name}",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        this.AvailableCertificates.Add(vm);
                    }
                }

                if (!foundPublished && publishedCert != null)
                {
                    var vm = this.certificate2ViewModelFactory.CreateViewModel(publishedCert);
                    vm.IsOrphaned = true;
                    vm.IsPublished = true;
                    this.AvailableCertificates.Add(vm);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(EventIDs.UIGenericError, ex, "Could not load certificate list");
                await this.dialogCoordinator.ShowMessageAsync(this, "Error", $"Could not refresh the certificate list\r\n{ex.Message}");
            }
        }

        public PackIconUniconsKind Icon => PackIconUniconsKind.Asterisk;
    }
}
