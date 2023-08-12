const fs = require('fs');
const path = require('path');
const { NFTStorage, File, Blob } = require('nft.storage');

// Your NFT.Storage API token
const NFT_STORAGE_TOKEN = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJkaWQ6ZXRocjoweEQ4RmZGNTk4ZjE5NjQ5NTY0RjRFZmY4RjBlNEVFYTU4NDQ2OUNGRjIiLCJpc3MiOiJuZnQtc3RvcmFnZSIsImlhdCI6MTY1NzMwMjQ2ODYwMywibmFtZSI6InRlbmsifQ.ZgqRfmAB3F_Sj6DCKZtGx-vPojcq_ZwLyyKyvbrnXnM';

// Function to read the content of a JSON file
function readJSONFile(filePath) {
  const fileContent = fs.readFileSync(filePath, 'utf-8');
  return JSON.parse(fileContent);
}

// Function to upload NFT files and generate metadata
async function uploadNFTFiles() {
  const client = new NFTStorage({ token: NFT_STORAGE_TOKEN });
  const resultUrls = [];

  // Read the files in the current directory
  const files = fs.readdirSync(__dirname);

  // Filter and process JSON files
  const jsonFiles = files.filter(file => path.extname(file) === '.json');
  for (const jsonFile of jsonFiles) {
    const baseFileName = path.basename(jsonFile, '.json');
    const glbFile = `${baseFileName}.glb`;
    const jpgFile = `${baseFileName}.jpg`;

    // Check if corresponding GLB and JPG files exist
    if (files.includes(glbFile) && files.includes(jpgFile)) {
      // Read the JSON file
      const jsonFilePath = path.join(__dirname, jsonFile);
      const metadata = readJSONFile(jsonFilePath);

      // Upload GLB file
      const glbFilePath = path.join(__dirname, glbFile);
      const glbFileData = new Blob([fs.readFileSync(glbFilePath)]);
      const glbCID = await client.storeBlob(glbFileData);
      metadata.animation_url = `https://ipfs.io/ipfs/${glbCID}`;

      // Upload JPG file
      const jpgFilePath = path.join(__dirname, jpgFile);
      const jpgFileData = new Blob([fs.readFileSync(jpgFilePath)]);
      const jpgCID = await client.storeBlob(jpgFileData);
      metadata.image = `https://ipfs.io/ipfs/${jpgCID}`;

      // Generate separate JSON file
      const nftMetadata = {
        description: metadata.description,
        external_url: 'https://ailand.app/',
        image: metadata.image,
        name: metadata.name,
        animation_url: metadata.animation_url,
        attributes: metadata.attributes,
      };
      const nftMetadataFileName = `${baseFileName}_metadata.json`;
      fs.writeFileSync(nftMetadataFileName, JSON.stringify(nftMetadata, null, 2));

      // Upload metadata file
      const nftMetadataFileData = new Blob([fs.readFileSync(nftMetadataFileName)]);
      const nftMetadataCID = await client.storeBlob(nftMetadataFileData);

      // Store the result URL
      resultUrls.push(`https://ipfs.io/ipfs/${nftMetadataCID}`);

      console.log(`NFT ${baseFileName} uploaded and metadata generated.`);
    }
  }

  console.log('All NFT files processed.');
  console.log('Result URLs:', resultUrls);

  // Save result URLs in a JSON file
  fs.writeFileSync('result_urls.json', JSON.stringify(resultUrls, null, 2));
}

uploadNFTFiles();
