const hre = require("hardhat");
const NUM_ITEMS = 1;

async function main() {
  const [signer] = await hre.ethers.getSigners();
  //console.log("Signer address:", signer.address);
  const signerBalance = await hre.ethers.provider.getBalance(signer.address);
  //console.log("Signer balance:", hre.ethers.utils.formatEther(signerBalance));

  const GameItem = await hre.ethers.getContractFactory("GameItem");
  //console.log(GameItem);

  const gameItem = await GameItem.attach("0xaC7e4Ad5d7557B78ebc84Dff668A06709f5Dc62B");

  for (var i = 1; i <= NUM_ITEMS; i++) {
    await gameItem.mintItem("https://gnfd-testnet-sp1.bnbchain.org/view/ailand-testnet/metadata.json");
  }
}

main()
  .then(() => process.exit(0))
  .catch((error) => {
    console.error(error);
    process.exit(1);
  });
