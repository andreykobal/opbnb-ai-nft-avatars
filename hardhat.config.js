require("@nomiclabs/hardhat-waffle");
require("dotenv").config();
require("@nomiclabs/hardhat-etherscan");

const privateKey = process.env.MNEMONIC
const maticUrl = process.env.MATIC_APP_ID
const polyScan = process.env.POLYGONSCAN
module.exports = {
  solidity: "0.8.0",
  networks: {
    matic: {
      chainId: 137,
      url: `https://rpc-mainnet.maticvigil.com/v1/${maticUrl}`,
      accounts: [privateKey]
    },
    mantle: {
      chainId: 5001,
      url: `https://rpc.testnet.mantle.xyz/`,
      accounts: [privateKey]
    },
    skaleTestnet: {
      chainId: 1351057110,
      url: `https://staging-v3.skalenodes.com/v1/staging-fast-active-bellatrix      `,
      accounts: [privateKey]
    },
      mymetaverse: {
      chainId: 1687098052079624,
      url: `https://mymetaverse-1687098052079624-1.jsonrpc.sp1.sagarpc.io/`,
      accounts: [privateKey]
    },
    gnosis: {
      url: "https://rpc.gnosischain.com",
      accounts: [privateKey],
    },
    chiado: {
      url: "https://rpc.chiadochain.net",
      gasPrice: 1000000000,
      accounts: [privateKey],
    },
    testnetbsc: {
      chainId: 97,
      url: "https://data-seed-prebsc-1-s1.bnbchain.org:8545",
      accounts: [privateKey],
      gasPrice: 10000000000,
    },
    opbnb: {
      chainId: 5611, 
      url: "https://opbnb-testnet.nodereal.io/v1/64a9df0874fb4a93b9d0a3849de012d3",
      accounts: [privateKey],
      gasPrice: 10000000000,
    },
    bnbGreenfield: {
      url: "https://gnfd-testnet-fullnode-tendermint-us.bnbchain.org",
      accounts: [privateKey],
      gasPrice: 10000000000,
    },
    auroraTestnet: {
      chainId: 1313161555,
      url: "https://testnet.aurora.dev",
      accounts: [privateKey],
    },
    meterTestnet: {
      chainId: 83,
      url: "https://rpctest.meter.io/",
      accounts: [privateKey],
    },
    xinfin: {
      url: process.env.XINFIN_NETWORK_URL,
      accounts: [privateKey],
    },
     apothem: {
      url: process.env.APOTHEM_NETWORK_URL,
      accounts: [privateKey]
    },
    goerli: {
      url: "https://rpc.ankr.com/eth_goerli",
      accounts: [privateKey]   
    },
    klaytnBaobab: {
      chainId: 1001,
      url: "https://public-en-baobab.klaytn.net",
      accounts: [privateKey]
    },
    mumbai: {
      chainId: 80001,
      url: "https://rpc-mumbai.maticvigil.com",
      accounts: [privateKey]
    }
  },
  //* Keep name as 'etherscan' to avoid errors.
  etherscan: {
    url: 'https://polygonscan.com/',
    apiKey: polyScan,
  }
};
