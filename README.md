# HackingStudio

HackingStudio is a powerful toolset for cyber enthusiasts and professionals, developed by Realtin (Coding Boson). It
provides a variety of functionalities to prepare and cook cyber recipes, leveraging advanced cryptographic techniques
and data manipulation steps.

## Features

- **Base64 Encoding/Decoding**: Encode and decode data using Base64.
- **Base58 Encoding/Decoding**: Encode and decode data using Base58.
- **Encryption/Decryption**: AES encryption and decryption.
- **Extensible Recipe System**: Create and execute custom recipes for data processing.

## Installation

To install HackingStudio as a .NET tool, run the following command:

```bash
git clone https://github.com/CodingBoson/HackingStudio.git
cd ./HackingStudio/HackingStudio.CLI/

./install.ps1 # For Windows
./install.sh # For Linux/MacOS
```

### Web interface (coming soon):

## Usage

### Hack a password using brute force:

```bash
hs brute find Your_Hash --algorithm SHA256 --list /path/to/words.lst --threads 16
```

### Cooking a Recipe

To cook a recipe, use the `cook` command:

- `-d`: The data to be processed.
- `-r`: The recipe file to use.
- `-o`: The output file to save the result (default is `stdout`).

### Listing Available Steps

To list all available steps for creating recipes, use the `steps` command:

### Checking a Recipe

To check the validity of a recipe, use the `check-recipe` command:

## Example Recipes

### Simple Recipe A

Encrypt data with AES and then encode it with Base64:

```xml
<Recipe>
	<AesEncrypt key="Your_Password"/>
	<Base64Encode />
</Recipe>
```

### Simple Recipe B

Decode data from Base64 then decrypt it with AES:

```xml
<Recipe>
	<Base64Decode />
	<AesDecrypt key="Your_Password"/>
</Recipe>
```

## Contributing

Contributions are welcome! Please fork the repository and submit a pull request.

## License

This project is licensed under the GPL-3.0 License.
