
https://www.dcode.fr/chiffre-base-58
https://www.youtube.com/results?search_query=base58

-----------------------

Base58 is a numbering system that uses 58 characters to represent numeric values. It is commonly used in applications where compact, human-readable, and less error-prone representations of data are needed, such as in cryptocurrencies and encoding identifiers.

### The Base58 Character Set:
The Base58 alphabet typically includes the following characters:

```
123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz
```

### Key Features:
1. **Excludes Similar Characters**:
   - Base58 omits characters that are visually similar and prone to confusion, such as:
     - `0` (zero) and `O` (uppercase letter O)
     - `I` (uppercase letter I) and `l` (lowercase letter L)
     - `+` and `/` (from Base64 encoding)
   
2. **No Padding**:
   - Unlike Base64, Base58 does not include padding characters (like `=`).

3. **Compact Representation**:
   - Base58 provides a more compact representation compared to hexadecimal or Base64 encoding.

### Applications:
1. **Cryptocurrency**:
   - Used to encode Bitcoin addresses (Base58Check encoding).
   - It helps make addresses more human-readable and reduces transcription errors.

2. **IPFS (InterPlanetary File System)**:
   - Base58 is used to encode content identifiers.

3. **Other Identifiers**:
   - Some systems use Base58 for unique user IDs, referral codes, or similar purposes.

### How Base58 Encoding Works:
1. **Convert the Input to an Integer**:
   - Treat the input data (binary or otherwise) as a single large integer.
   
2. **Repeated Division by 58**:
   - Divide the integer repeatedly by 58 and record the remainder each time.

3. **Map Remainders to the Base58 Alphabet**:
   - Use the remainders to look up corresponding characters in the Base58 alphabet.

4. **Handle Leading Zero Bytes**:
   - Leading zeros in the input are encoded as `1` in Base58 (since `1` is the first character in the Base58 alphabet).

### Example:
Suppose you want to encode the number `1000` in Base58:

1. Convert `1000` into Base58:
   - `1000 ÷ 58 = 17 remainder 16`
   - `17 ÷ 58 = 0 remainder 17`

2. Map the remainders (16, 17) to the Base58 alphabet:
   - 16 → `H`, 17 → `J`.

3. Combine the results:
   - `1000` in Base58 is `JH`.

Let me know if you'd like a more detailed example or an explanation of a specific use case!