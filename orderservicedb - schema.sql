-- Tabla: Products
CREATE TABLE Products (
    ProductId CHAR(36) PRIMARY KEY, -- Identificador único del producto (UUID en formato texto)
    SKU VARCHAR(50) NOT NULL UNIQUE, -- Stock Keeping Unit
    InternalCode VARCHAR(50) NOT NULL UNIQUE, -- Código interno del producto
    Name VARCHAR(255) NOT NULL, -- Nombre del producto
    Description TEXT NULL, -- Descripción larga
    CurrentUnitPrice DECIMAL(18, 2) NOT NULL, -- Precio unitario actual
    StockQuantity INT NOT NULL DEFAULT 0 -- Cantidad en stock
);

-- Tabla: Orders
CREATE TABLE Orders (
    OrderId CHAR(36) PRIMARY KEY, -- Identificador único de la orden (UUID)
    CustomerId CHAR(36) NOT NULL, -- ID del cliente (simulado)
    OrderDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP, -- Fecha de orden
    OrderStatus VARCHAR(50) NOT NULL, -- Estado de la orden
    TotalAmount DECIMAL(18, 2) NOT NULL, -- Total de la orden
    ShippingAddress TEXT NOT NULL, -- Dirección de envío
    BillingAddress TEXT NOT NULL, -- Dirección de facturación
    Notes TEXT NULL -- Notas adicionales
);

-- Tabla: OrderItems
CREATE TABLE OrderItems (
    OrderItemId CHAR(36) PRIMARY KEY, -- Identificador del ítem (UUID)
    OrderId CHAR(36) NOT NULL, -- FK a Orders
    ProductId CHAR(36) NOT NULL, -- FK a Products
    Quantity INT NOT NULL, -- Cantidad
    UnitPrice DECIMAL(18, 2) NOT NULL, -- Precio al momento de la orden
    Subtotal DECIMAL(18, 2) NOT NULL, -- Subtotal (Quantity * UnitPrice),
    FOREIGN KEY (OrderId) REFERENCES Orders(OrderId),
    FOREIGN KEY (ProductId) REFERENCES Products(ProductId)
);