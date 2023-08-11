const hre = require("hardhat");

async function main() {
  const GameItem = await hre.ethers.getContractFactory("GameItems");
  const gameItem = await GameItem.attach('0x643D5cf6fdd9Faa3825e194e925D07E290E993D2');

  const account = "0x204f9781DDcafB4a844fd12250dB15183C67cACB";
  const totalBalance = await gameItem.getTotalBalance(account);

  console.log(`Total balance for ${account}: ${totalBalance}`);
}

main()
  .then(() => process.exit(0))
  .catch((error) => {
    console.error(error);
    process.exit(1);
  });
