const hre = require("hardhat");

async function main() {
    const GameItem = await hre.ethers.getContractFactory("GameItems");
    const gameItem = await GameItem.attach('0xd68B7C666b269B3FC9daAc7a3a446bE32999920E');

    const NUM_ITEMS = 1;
    const tokenURIs = [
        "https://gnfd-testnet-sp1.bnbchain.org/view/ailand-testnet/metadata.json"
    ];

    for (let i = 0; i < NUM_ITEMS; i++) {
        console.log(`Minting tokens: Batch ${i + 1}`);
        await gameItem.batchMint(tokenURIs);
    }
}

main()
    .then(() => process.exit(0))
    .catch((error) => {
        console.error(error);
        process.exit(1);
    });
