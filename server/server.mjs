// ==========================================
// Imports
// ==========================================

import http from 'node:http';
import fs from 'node:fs';
import path from 'node:path';
import { fileURLToPath } from 'node:url';

// ==========================================
// Paths
// ==========================================

const __filename = fileURLToPath(import.meta.url);
const __dirname = path.dirname(__filename);

// ==========================================
// Configuration
// ==========================================

const PORT = 4050;

// These are the ONLY codes that will work.
// Base64 encoding does not make a code valid.
const VALID_ENCRYPTION_CODES = [
    'MARKETPLACE-2026',
    'CBX-MARKET-ACCESS',
    'DEMO-ACCESS-123'
];

// ==========================================
// File helpers
// ==========================================

const PRODUCTS_FILE = path.join(__dirname, 'products.json');
const PURCHASES_FILE = path.join(__dirname, 'purchases.json');

function loadProducts() {
    try {
        return JSON.parse(fs.readFileSync(PRODUCTS_FILE, 'utf8'));
    } catch (error) {
        console.error('[Products] Failed to load products:', error.message);
        return [];
    }
}

function saveProducts(products) {
    fs.writeFileSync(
        PRODUCTS_FILE,
        JSON.stringify(products, null, 2),
        'utf8'
    );
}

function loadPurchases() {
    try {
        if (!fs.existsSync(PURCHASES_FILE)) {
            return [];
        }

        return JSON.parse(fs.readFileSync(PURCHASES_FILE, 'utf8'));
    } catch (error) {
        console.error('[Purchases] Failed to load purchases:', error.message);
        return [];
    }
}

function savePurchases(purchases) {
    fs.writeFileSync(
        PURCHASES_FILE,
        JSON.stringify(purchases, null, 2),
        'utf8'
    );
}

// ==========================================
// Base64 helpers
// ==========================================

function encodeBase64(value) {
    return Buffer
        .from(JSON.stringify(value), 'utf8')
        .toString('base64');
}

function decodeBase64(value) {
    try {
        return JSON.parse(
            Buffer.from(value, 'base64').toString('utf8')
        );
    } catch {
        return null;
    }
}

// ==========================================
// HTTP helpers
// ==========================================

function sendJson(res, statusCode, data) {
    const response = JSON.stringify(data);

    res.writeHead(statusCode, {
        'Content-Type': 'application/json',
        'Access-Control-Allow-Origin': '*'
    });

    res.end(response);
}

function sendBase64Response(res, statusCode, data) {
    sendJson(res, statusCode, {
        encoded: encodeBase64(data)
    });
}

function readRequestBody(req) {
    return new Promise((resolve, reject) => {
        let body = '';

        req.on('data', chunk => {
            body += chunk;
        });

        req.on('end', () => {
            resolve(body);
        });

        req.on('error', reject);
    });
}

// ==========================================
// Server
// ==========================================

const server = http.createServer(async (req, res) => {
    const requestUrl = new URL(
        req.url,
        `http://${req.headers.host || 'localhost'}`
    );

    const pathname = requestUrl.pathname;

    console.log(`[HTTP] ${req.method} ${pathname}`);

    // ======================================
    // CORS preflight
    // ======================================

    if (req.method === 'OPTIONS') {
        res.writeHead(204, {
            'Access-Control-Allow-Origin': '*',
            'Access-Control-Allow-Methods': 'GET, POST, OPTIONS',
            'Access-Control-Allow-Headers': 'Content-Type'
        });

        return res.end();
    }

    // ======================================
    // Serve product images
    // ======================================

    if (req.method === 'GET' && pathname.startsWith('/images/')) {
        const requestedFile = path.basename(pathname);
        const imagePath = path.join(__dirname, 'images', requestedFile);

        if (!fs.existsSync(imagePath)) {
            return sendJson(res, 404, {
                error: 'Image not found.'
            });
        }

        const extension = path.extname(imagePath).toLowerCase();

        const contentTypes = {
            '.jpg': 'image/jpeg',
            '.jpeg': 'image/jpeg',
            '.png': 'image/png',
            '.gif': 'image/gif',
            '.webp': 'image/webp'
        };

        const contentType = contentTypes[extension] || 'application/octet-stream';

        res.writeHead(200, {
            'Content-Type': contentType
        });

        return fs.createReadStream(imagePath).pipe(res);
    }

    // ======================================
    // Health check
    // ======================================

    if (req.method === 'GET' && pathname === '/') {
        return sendJson(res, 200, {
            name: 'Marketplace Server',
            status: 'online',
            version: '1.0.0'
        });
    }

    // ======================================
    // Authenticate client
    // ======================================

    if (
        req.method === 'POST' &&
        pathname === '/api/authenticate'
    ) {
        try {
            const body = JSON.parse(await readRequestBody(req));

            const decoded = decodeBase64(body.encoded);

            if (!decoded || typeof decoded.code !== 'string') {
                return sendBase64Response(res, 400, {
                    success: false,
                    message: 'Invalid encoded authentication request.'
                });
            }

            const suppliedCode = decoded.code;

            console.log('[Auth] Received access code.');

            if (!VALID_ENCRYPTION_CODES.includes(suppliedCode)) {
                console.log('[Auth] Invalid access code.');

                return sendBase64Response(res, 401, {
                    success: false,
                    message: 'Invalid encryption/access code.'
                });
            }

            console.log('[Auth] Client authenticated.');

            return sendBase64Response(res, 200, {
                success: true,
                message: 'Authentication successful.',
                products: loadProducts()
            });

        } catch (error) {
            console.error('[Auth] Error:', error.message);

            return sendBase64Response(res, 500, {
                success: false,
                message: 'Authentication server error.'
            });
        }
    }

    // ======================================
    // Product list
    // ======================================

    if (
        req.method === 'POST' &&
        pathname === '/api/products'
    ) {
        try {
            const body = JSON.parse(await readRequestBody(req));

            const decoded = decodeBase64(body.encoded);

            if (!decoded || !decoded.authenticated) {
                return sendBase64Response(res, 401, {
                    success: false,
                    message: 'Authentication required.'
                });
            }

            return sendBase64Response(res, 200, {
                success: true,
                products: loadProducts()
            });

        } catch (error) {
            return sendBase64Response(res, 400, {
                success: false,
                message: 'Invalid product request.'
            });
        }
    }

    // ======================================
    // Like / dislike
    // ======================================

    if (
        req.method === 'POST' &&
        pathname === '/api/reaction'
    ) {
        try {
            const body = JSON.parse(await readRequestBody(req));

            const decoded = decodeBase64(body.encoded);

            if (
                !decoded ||
                !decoded.authenticated ||
                !Number.isInteger(decoded.productId) ||
                !['like', 'dislike'].includes(decoded.reaction)
            ) {
                return sendBase64Response(res, 400, {
                    success: false,
                    message: 'Invalid reaction request.'
                });
            }

            const products = loadProducts();

            const product = products.find(
                item => item.id === decoded.productId
            );

            if (!product) {
                return sendBase64Response(res, 404, {
                    success: false,
                    message: 'Product not found.'
                });
            }

            if (decoded.reaction === 'like') {
                product.likes += 1;
            } else {
                product.dislikes += 1;
            }

            saveProducts(products);

            console.log(
                `[Reaction] Product ${product.id}: ${decoded.reaction}`
            );

            return sendBase64Response(res, 200, {
                success: true,
                message: `${decoded.reaction} recorded.`,
                likes: product.likes,
                dislikes: product.dislikes
            });

        } catch (error) {
            return sendBase64Response(res, 500, {
                success: false,
                message: 'Reaction server error.'
            });
        }
    }

    // ======================================
    // Log purchase
    // ======================================

    if (
        req.method === 'POST' &&
        pathname === '/api/purchase'
    ) {
        try {
            const body = JSON.parse(await readRequestBody(req));

            const decoded = decodeBase64(body.encoded);

            if (
                !decoded ||
                !decoded.authenticated ||
                !Number.isInteger(decoded.productId) ||
                !Number.isInteger(decoded.quantity) ||
                decoded.quantity <= 0
            ) {
                return sendBase64Response(res, 400, {
                    success: false,
                    message: 'Invalid purchase request.'
                });
            }

            const products = loadProducts();

            const product = products.find(
                item => item.id === decoded.productId
            );

            if (!product) {
                return sendBase64Response(res, 404, {
                    success: false,
                    message: 'Product not found.'
                });
            }

            if (decoded.quantity > product.stock) {
                return sendBase64Response(res, 400, {
                    success: false,
                    message: `Only ${product.stock} item(s) available.`
                });
            }

            const total = Number(
                (product.price * decoded.quantity).toFixed(2)
            );

            const purchase = {
                id: Date.now(),
                productId: product.id,
                productTitle: product.title,
                quantity: decoded.quantity,
                total,
                customerInformation: decoded.information || '',
                timestamp: new Date().toISOString()
            };

            const purchases = loadPurchases();
            purchases.push(purchase);
            savePurchases(purchases);

            // Reduce stock after purchase.
            product.stock -= decoded.quantity;
            saveProducts(products);

            console.log(
                `[Purchase] ${product.title} x${decoded.quantity} = $${total}`
            );

            return sendBase64Response(res, 200, {
                success: true,
                message: 'Purchase logged successfully.',
                purchaseId: purchase.id,
                total,
                remainingStock: product.stock
            });

        } catch (error) {
            console.error('[Purchase] Error:', error.message);

            return sendBase64Response(res, 500, {
                success: false,
                message: 'Purchase server error.'
            });
        }
    }

    // ======================================
    // Unknown route
    // ======================================

    return sendJson(res, 404, {
        error: 'Route not found.'
    });
});

// ==========================================
// Start server
// ==========================================

server.listen(PORT, () => {
    console.log(`Marketplace server running at http://localhost:${PORT}`);
});