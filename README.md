
# 🛒 OrderServiceAPI

es una API RESTful desarrollada en ASP.NET Core 8 + Entity Framework Core + MySQL. Permite la gestión de órdenes de compra en una tienda en línea. Forma parte de un sistema de e-commerce en desarrollo y se encarga exclusivamente del registro, consulta y actualización de pedidos (sin incluir lógica de pagos).

---

## 🚀 Tecnologías utilizadas

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core 8.0.6
- MySQL 8.x
- Pomelo.EntityFrameworkCore.MySql
- Swagger (Swashbuckle)

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

�?Verifica stock, calcula subtotales, guarda orden y descuenta el stock.

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

## 🧪 Endpoint de prueba para insertar productos (opcional)

**POST** `/api/seed`  
Inserta productos ficticios para pruebas.

---

## 🧠 Estructura del proyecto

```
OrderServiceAPI/
├── Controllers/
├── DTOs/
├── Models/
├── Services/
├── Data/
├── Program.cs
└── appsettings.json
```

---

## �?Checklist del TP

- [x] Crear orden con control de stock
- [x] Obtener orden individual
- [x] Listar órdenes con filtros
- [x] Cambiar estado de orden
- [x] EF Core con migraciones y MySQL
- [x] Código limpio y organizado
- [x] Swagger activo

---

## 👨‍�?Información académica

- **Alumno**: Lastra Mario, Barranquero Germán, Cruz Gareca Adólfo
- **Carrera**: Ingeniería en Sistemas
- **Universidad**: UTN �?Facultad Regional Tucumán
- **Materia**: Desarrollo de Software
- **Año**: 2025

---
