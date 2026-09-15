# Marketplace App Server Fullstack

A small full-stack demo project with:

- A Node.js HTTP API server (`/server/server.mjs`)
- A Windows Forms desktop client (`/app`)

The client communicates with the server using Base64-encoded JSON payloads for authentication, product browsing, reactions, and purchases.

## Project structure

- `/server` – Node.js backend server
- `/app` – .NET Windows Forms frontend
- `package.json` – Node scripts and dependencies

## Prerequisites

- Node.js 18+ (recommended)
- npm
- .NET SDK (Windows, target: `net10.0-windows`)

## Backend setup and run

From the repository root:

```bash
npm install
npm start
```

The server runs at:

- `http://localhost:4050`

## Frontend setup and run

Open `/app/app.slnx` (or `/app/app.csproj`) in Visual Studio on Windows, then run the app.

The client is configured to call:

- `http://localhost:4050`

## API overview

- `GET /` – health/status
- `POST /api/authenticate` – authenticate and return products
- `POST /api/products` – fetch product list (authenticated)
- `POST /api/reaction` – submit like/dislike
- `POST /api/purchase` – submit purchase
- `GET /images/:file` – serve product image files

## Demo authentication codes

The server currently accepts these access codes:

- `MARKETPLACE-2026`
- `CBX-MARKET-ACCESS`
- `DEMO-ACCESS-123`

## Notes

- `products.json` should be created and maintained under `/server`.
- Example `products.json`:

```json
[
  {
    "id": 1,
    "category": "Computers",
    "title": "Laptop Computer - $899.99",
    "description": "A general-purpose laptop computer.",
    "price": 899.99,
    "stock": 9,
    "image": "http://localhost:4050/images/laptop.jpg",
    "likes": 1,
    "dislikes": 0
  }
]
```

- `purchases.json` is created/updated automatically by the server when purchases are logged.
- Product images should be stored in `/server/images`.
- This project is intended as a learning/demo environment and not production hardened.
