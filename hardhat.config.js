require("@nomiclabs/hardhat-waffle");
require("dotenv").config();
require("@nomiclabs/hardhat-etherscan");

const privateKey = process.env.MNEMONIC
const maticUrl = process.env.MATIC_APP_ID
const polyScan = process.env.POLYGONSCAN
module.exports = {
  solidity: "0.8.0",
  networks: {
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
    goerli: {
      url: "https://rpc.ankr.com/eth_goerli",
      accounts: [privateKey]   
    }
  },
  //* Keep name as 'etherscan' to avoid errors.
  etherscan: {
    url: 'https://polygonscan.com/',
    apiKey: polyScan,
  }
};
