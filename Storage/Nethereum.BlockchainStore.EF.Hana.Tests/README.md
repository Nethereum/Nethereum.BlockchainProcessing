# Building Nethereum.BlockchainStore.EF.Hana.Tests

Note that the file `App.config` is hidden from build since it is named `App.config.HIDEME`. 
This project will build as-is using default EF data provider settings, not HANA.

To build for HANA:

1. Install HANA data provider on the same PC that build will happen. Instructions here: https://help.sap.com/viewer/1efad1691c1f496b8b580064a6536c2d/Cloud/en-US/7017cce72a054111b71cc713762e365c.html
2. Rename  `App.config.HIDEME` to be `App.config`.
3. Build project `Nethereum.BlockchainStore.EF.Hana.Tests`.

The build should complete successfully.



