# Building HANA Console Program: Nethereum.BlockchainStore.EF.Hana.Console

Note that the file `App.config` is hidden from the build process since it is named `App.config.EXAMPLE`. 
This project will build as-is using default EF data provider settings, not HANA.

To build for HANA:

1. Install HANA data provider on the same PC that build will happen. Instructions here: https://help.sap.com/viewer/1efad1691c1f496b8b580064a6536c2d/Cloud/en-US/7017cce72a054111b71cc713762e365c.html
2. Rename  `App.config.EXAMPLE` to be `App.config`.
3. Edit the `App.config` file to have the correct connection string for your HANA system. Note the port number is `3<instance>13`. For example, to connect to a HANA system with instance 90 you should specify port `39013` in the connection string.
4. Build project `Nethereum.BlockchainStore.EF.Hana.Console`.

The build should complete successfully.


# Running HANA Console Program: Nethereum.BlockchainStore.EF.Hana.Console

1. A HANA 2.x system is required.
2. Ensure that the schema `dbo` exists on the HANA system. This schema is required by EF to store an admin table.
3. Ensure that the schema `MAINNET` exists in the HANA sysetem. This is where the blockchain data will be stored.
4. Open a command prompt in folder `\Nethereum.BlockchainProcessing\Storage\Nethereum.BlockchainStore.EF.Hana.Console\bin\Debug`.
5. Data is written to the HANA tables in batches consisting of ranges of `blocks` from the blockchain. The sample command below will process 100 blocks from 8026600 to 802699. Notice how we can specify the source blockchain URL and the target schema  on HANA:

```
> Nethereum.BlockchainStore.EF.Hana.Console --HanaSchema MAINNET --FromBlock 8026640 --ToBlock 8026642 --BlockchainUrl "https://mainnet.infura.io/"
```
6. After some time, output like below will show progress:
```info: Nethereum.BlockchainStore.EF.Hana.Console.Program[0]
      Begin FillContractCacheAsync
info: Nethereum.BlockchainStore.EF.Hana.Console.Program[0]
      Beginning Block Enumeration
info: Nethereum.BlockchainStore.EF.Hana.Console.Program[0]
      Block Process Attempt.  Block: 8026640, Attempt: 0.
info: Nethereum.BlockchainStore.EF.Hana.Console.Program[0]
      Block Incremented, Current Block: 8026641
info: Nethereum.BlockchainStore.EF.Hana.Console.Program[0]
      Block Process Attempt.  Block: 8026641, Attempt: 0.
info: Nethereum.BlockchainStore.EF.Hana.Console.Program[0]
      Block Incremented, Current Block: 8026642
info: Nethereum.BlockchainStore.EF.Hana.Console.Program[0]
      Block Process Attempt.  Block: 8026642, Attempt: 0.
Duration: 00:13:34.8894336
```