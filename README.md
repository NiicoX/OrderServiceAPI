# 🛒 OrderServiceAPI

Es una API RESTful desarrollada en ASP.NET Core 8 + Entity Framework Core + MySQL. Permite la gestión de órdenes de compra en una tienda en línea. Forma parte de un sistema de e-commerce en desarrollo y se encarga exclusivamente del registro, consulta y actualización de pedidos (sin incluir lógica de pagos).

---

## 🚀 Tecnologías utilizadas

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core 8.0.6
- MySQL 8.x
- Pomelo.EntityFrameworkCore.MySql
- Swagger (Swashbuckle) + JWT Auth
- xUnit (tests unitarios)

---

## ⚙️ Configuración del proyecto

### 1. Requisitos

- Visual Studio 2022+
- .NET SDK 8.0
- MySQL Server instalado
- CLI de EF Core: `dotnet tool install --global dotnet-ef`

---

### 2. Clonación y cadena de conexión

```bash
git clone https://github.com/tu-usuario/OrderServiceAPI.git
cd OrderServiceAPI
```

Editá `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=OrderServiceDB;User=root;Password=tu_contraseña;"
},
"Jwt": {
  "Key": "clave_secreta_segura", // <--- Cadena de 32 digitos obligatoria
  "Issuer": "OrderServiceAPI",
  "Audience": "OrderServiceAPIUsers"
}
```

---

### 3. Crear la base de datos

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

---

### 4. Ejecutar la aplicación

```bash
dotnet run
```

Ingresá a Swagger:  
📍 `https://localhost:{puerto}/swagger`

---

## 🔑 Autenticación
Login de usuario

**POST** `/api/auth/login`
```json
{
  "username": "admin",
  "password": "1234"
}
```

## 📦 Endpoints implementados

### 🔹 Crear una orden

**POST** `/api/order`

```json
{
  "customerId": "11113333-2222-1413-1010-134415516099",
  "shippingAddress": "San Martin 38",
  "billingAddress": "San Martin 38",
  "orderItems": [
    {
      "productId": "GUID_PRODUCTO",
      "quantity": 2,
      "unitPrice": 8500.00
    }
  ]
}
```

✔️ Verifica stock, calcula subtotales, guarda orden y descuenta inventario.

---

### 🔹 Listar todas las órdenes

**GET** `/api/order`

Parámetros opcionales:

- `status=Pending`
- `customerId={GUID}`
- `pageNumber=1`
- `pageSize=10`

---

### 🔹 Obtener una orden por ID

**GET** `/api/order/{id}`

---

### 🔹 Actualizar estado de una orden

**PUT** `/api/order/{id}/status`

```json
{
  "newStatus": "Shipped"
}
```

Estados válidos:
- `Pending`
- `Processing`
- `Shipped`
- `Delivered`
- `Cancelled`

---

## 🧪 Tests unitarios (almacenamiento en memoria y no en db)
El proyecto incluye pruebas con xUnit y InMemoryDbContext:
```bash
dotnet test
```

🔹 Endpoint de prueba para insertar productos (opcional)
**POST** `/api/seed`
Inserta productos ficticios para pruebas en la base de datos.

---

## 🧠 Estructura del proyecto

```
OrderServiceAPI/
├── Core/               # Entidades y DTOs
├── Infrastructure/     # Servicios, Data, Seguridad
├── Controllers/        # Controladores Web API
├── Tests/              # Pruebas unitarias con xUnit
├── Program.cs
└── appsettings.json
```
