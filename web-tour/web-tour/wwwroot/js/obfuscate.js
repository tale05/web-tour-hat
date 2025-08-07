// node obfuscate.js

const JavaScriptObfuscator = require('javascript-obfuscator');
const fs = require('fs');
const path = require('path');

// Khai báo đường dẫn tuyệt đối đến thư mục js
const baseDir = 'D:\\WebsiteTour\\hosting\\web-tour\\web-tour\\wwwroot\\js';

// Tạo đường dẫn tuyệt đối tới file cần đọc và file đầu ra

//const inputPath = path.join(baseDir, 'news', 'login.js');
//const outputPath = path.join(baseDir, 'news', 'login.obfuscated.js');

const inputPath = path.join(baseDir, 'login.js');
const outputPath = path.join(baseDir, 'login.obfuscated.js');

// Đọc file js
const jsCode = fs.readFileSync(inputPath, 'utf8');

// Thực hiện obfuscate
const obfuscatedCode = JavaScriptObfuscator.obfuscate(jsCode, {
    compact: true,
    controlFlowFlattening: true,
    controlFlowFlatteningThreshold: 0.75,
    numbersToExpressions: true,
    simplify: true,
    shuffleStringArray: true,
    splitStrings: true,
    stringArrayThreshold: 0.75
}).getObfuscatedCode();

// Ghi file kết quả
fs.writeFileSync(outputPath, obfuscatedCode);

console.log('JS file obfuscated successfully!');