
# Hướng dẫn sử dụng HashIdHelper trong ASP.NET

## 1. Tải thư viện HashIdHelper từ NuGet
- Sử dụng lệnh:
  ```
  Install-Package Hashids.net
  ```

## 2. Cấu hình HashIdHelper trong `appsettings.json`
```json
"HashIdSettings": {
  "Salt": "vF#9dWk3@Lzq8Xe1$RbTpZ7N&GmYH5CU",
  "MinLength": 12
}
```

## 3. Tạo class `HashIdHelper` trong folder `Helpers`
Tạo class như sau:
```csharp
using HashidsNet;
using Microsoft.Extensions.Configuration;

namespace web_tour.Helpers
{
    public class HashIdHelper
    {
        private readonly Hashids _hashids;

        public HashIdHelper(IConfiguration configuration)
        {
            var salt = configuration["HashIdSettings:Salt"] ?? "default_salt";
            var minLength = int.TryParse(configuration["HashIdSettings:MinLength"], out var length) ? length : 12;
            _hashids = new Hashids(salt, minLength);
        }

        public string EncodeId(int id) => _hashids.Encode(id);

        public int DecodeId(string encoded)
        {
            var result = _hashids.Decode(encoded);
            return result.Length > 0 ? result[0] : 0;
        }
    }
}
```

## 4. Sử dụng `HashIdHelper` trong các controller
- Inject `HashIdHelper` vào constructor của controller và sử dụng các phương thức `EncodeId` và `DecodeId`.
- Inject `HashIdHelper` vào controller: **dòng 14**
- Sử dụng phương thức `DecodeId`: **dòng 86**

## 5. Sử dụng `HashIdHelper` trong views
- Inject `HashIdHelper` vào view bằng cách:
  ```razor
  @inject web_tour.Helpers.HashIdHelper HashId
  ```
- Sử dụng hàm `EncodeId` trong view để mã hóa ID:
  ```csharp
  var encodedId = HashId.EncodeId(item.IdNews ?? 0);
  ```
