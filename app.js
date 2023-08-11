// index.js
const express = require("express");
const { ethers } = require("hardhat");
const { SDK, Auth, TEMPLATES, Metadata } = require("@infura/sdk");
require("dotenv").config();

const app = express();
const PORT = process.env.PORT || 3000;

app.use(express.json());

// Create Auth object
const auth = new Auth({
  projectId: process.env.INFURA_API_KEY,
  secretId: process.env.INFURA_API_KEY_SECRET,
  privateKey: process.env.WALLET_PRIVATE_KEY,
  rpcUrl: process.env.EVM_RPC_URL,
  chainId: 137, // Polygon
});

// Instantiate SDK
const sdk = new SDK(auth);

const CONTRACT_ADDRESS = "0xe3515d63BCE48059146134176DBB18B9Db0D80D8";

async function mintItem(ownerAddress, tokenURI) {
  const GameItem = await ethers.getContractFactory("GameItem");
  const gameItem = await GameItem.attach(CONTRACT_ADDRESS);

  return gameItem.mintItem(ownerAddress, tokenURI);
}

app.post("/mint", async (req, res) => {
  try {
    const { ownerAddress, tokenURI } = req.body;

    if (!ownerAddress || !tokenURI) {
      return res.status(400).json({ error: "Missing required fields" });
    }

    const result = await mintItem(ownerAddress, tokenURI);
    res.status(200).json({ result });
  } catch (error) {
    console.error(error);
    res.status(500).json({ error: "Internal server error" });
  }
});

app.get("/tokenMetadata", async (req, res) => {
  const { contractAddress, walletAddress } = req.query;

  if (!contractAddress || !walletAddress) {
    return res.status(400).send("contractAddress and walletAddress are required");
  }

  try {
    const mynfts = await sdk.api.getNFTs({ publicAddress: walletAddress });
    const metadataPromises = [];

    for (const nft of mynfts.assets) {
      if (nft.contract === contractAddress) {
        metadataPromises.push(
          sdk.api.getTokenMetadata({
            contractAddress: nft.contract,
            tokenId: nft.tokenId,
          })
        );
      }
    }

    const metadataArray = await Promise.all(metadataPromises);
    res.status(200).json(metadataArray);
  } catch (error) {
    console.log(error);
    res.status(500).send("Error fetching token metadata");
  }
});

app.listen(PORT, () => {
  console.log(`Server is running on port ${PORT}`);
});
