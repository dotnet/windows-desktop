ClickOnce Manifests were created with [Mage](https://docs.microsoft.com/visualstudio/deployment/walkthrough-manually-deploying-a-clickonce-application).

Steps followed:

1. Create version subdirectory (eg. `1.0.0.0`).
1. Copy application assets into the version directory.
1. From the version subdirectory, create the application manifest:
    1. `mage -New Application -Processor msil -IconFile BeanTrader.ico -ToFile BeanTrader.exe.manifest -name "Bean Trader" -Version 1.0.0.0 -FromDirectory .`
1. Sign the application manifest:
    1. `mage -Sign BeanTrader.exe.manifest -CertFile ..\BeanTraderClient.pfx`
1. From the root `ClickOnceManifest` directory, create the deployment manifest:
    1. `mage -New Deployment -Processor msil -Install true -Publisher "Mike Rousos" -ProviderUrl "https://beantrader.blob.core.windows.net/install/BeanTrader.application" -AppManifest 1.0.0.0\BeanTrader.exe.manifest -Version 1.0.0.0 -ToFile BeanTrader.application`
1. Adjusted deployment manifest's [`<subscription>` element](https://docs.microsoft.com/visualstudio/deployment/choosing-a-clickonce-update-strategy?view=vs-2017#check-for-updates-before-application-startup) to use 'beforeApplicationStartup' updates.
1. Sign the deployment manifest:
    1. `mage -sign BeanTrader.application -CertFile BeanTraderClient.pfx`

The contents of the ClickOnceManifests folder can then be uploaded to the deployment location (Azure storage, as indicated by the ProviderUrl parameter, in this case).

To update deployment manifest:
1. `mage -Update BeanTrader.application -Processor msil -Install true -Publisher "Mike Rousos" -ProviderUrl "https://beantrader.blob.core.windows.net/install/BeanTrader.application" -AppManifest 2.0.0.0\BeanTrader.exe.manifest -Version 2.0.0.0`
1. Adjusted deployment manifest's [`<subscription>` element](https://docs.microsoft.com/visualstudio/deployment/choosing-a-clickonce-update-strategy?view=vs-2017#check-for-updates-before-application-startup) to use 'beforeApplicationStartup' updates.
1. Sign the deployment manifest:
    1. `mage -sign BeanTrader.application -CertFile BeanTraderClient.pfx`