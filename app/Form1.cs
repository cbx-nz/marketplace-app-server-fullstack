using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace app
{
    public partial class Form1 : Form
    {
        // ==========================================
        // Server configuration
        // ==========================================

        private const string ServerUrl = "http://localhost:4050";

        private readonly HttpClient httpClient =
            new HttpClient();

        // ==========================================
        // Application state
        // ==========================================

        private bool isAuthenticated = false;

        private int selectedProductId = -1;

        private string selectedProductImageUrl = "";

        private readonly List<Product> products =
            new List<Product>();

        // ==========================================
        // Local reaction storage
        // ==========================================

        /*
         * This is the WinForms equivalent of localStorage.
         *
         * The file is stored inside:
         *
         * C:\Users\<username>\AppData\Local\MarketplaceApp\
         *
         * Each Windows user gets their own file.
         */

        private readonly string reactionsFilePath =
            Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData
                ),
                "MarketplaceApp",
                "reactions.json"
            );

        private List<LocalReaction> localReactions =
            new List<LocalReaction>();

        // ==========================================
        // Constructor
        // ==========================================

        public Form1()
        {
            InitializeComponent();

            // Connect button events.
            buttonUseEncryptionCode.Click +=
                buttonUseEncryptionCode_Click;

            buttonLoadProducts.Click +=
                buttonLoadProducts_Click;

            buttonLoadImage.Click +=
                buttonLoadImage_Click;

            buttonPlusOne.Click +=
                buttonPlusOne_Click;

            buttonMinusOne.Click +=
                buttonMinusOne_Click;

            buttonLogPurchase.Click +=
                buttonLogPurchase_Click;

            // Connect TreeView selection event.
            listBoxProducts.AfterSelect +=
                listBoxProducts_AfterSelect;

            // Load previously saved local reactions.
            LoadLocalReactions();

            // Initial UI state.
            buttonLoadProducts.Enabled = false;
            buttonLoadImage.Enabled = false;
            buttonPlusOne.Enabled = false;
            buttonMinusOne.Enabled = false;
            buttonLogPurchase.Enabled = false;

            textReadProductTitle.ReadOnly = true;

            textBoxQuantity.Text = "1";
        }

        // ==========================================
        // Data models
        // ==========================================

        public class Product
        {
            public int id { get; set; }

            public string category { get; set; } = "";

            public string title { get; set; } = "";

            public string description { get; set; } = "";

            public decimal price { get; set; }

            public int stock { get; set; }

            public string? image { get; set; }

            public int likes { get; set; }

            public int dislikes { get; set; }
        }

        public class ServerResponse
        {
            public bool success { get; set; }

            public string message { get; set; } = "";

            public List<Product>? products { get; set; }

            public int likes { get; set; }

            public int dislikes { get; set; }

            public long purchaseId { get; set; }

            public decimal total { get; set; }

            public int remainingStock { get; set; }
        }

        // ==========================================
        // Local reaction model
        // ==========================================

        public class LocalReaction
        {
            public int ProductId { get; set; }

            public string Reaction { get; set; } = "";
        }

        // ==========================================
        // Base64 helpers
        // ==========================================

        private string EncodeBase64(object data)
        {
            string json =
                JsonSerializer.Serialize(data);

            byte[] bytes =
                Encoding.UTF8.GetBytes(json);

            return Convert.ToBase64String(bytes);
        }

        private T? DecodeBase64<T>(string encoded)
        {
            try
            {
                byte[] bytes =
                    Convert.FromBase64String(encoded);

                string json =
                    Encoding.UTF8.GetString(bytes);

                return JsonSerializer.Deserialize<T>(json);
            }
            catch
            {
                return default;
            }
        }

        // ==========================================
        // HTTP helper
        // ==========================================

        private async Task<ServerResponse?> SendBase64RequestAsync(
            string endpoint,
            object requestData)
        {
            try
            {
                string encoded =
                    EncodeBase64(requestData);

                var requestBody = new
                {
                    encoded = encoded
                };

                string json =
                    JsonSerializer.Serialize(requestBody);

                using var content =
                    new StringContent(
                        json,
                        Encoding.UTF8,
                        "application/json"
                    );

                HttpResponseMessage response =
                    await httpClient.PostAsync(
                        ServerUrl + endpoint,
                        content
                    );

                string responseJson =
                    await response.Content.ReadAsStringAsync();

                using JsonDocument document =
                    JsonDocument.Parse(responseJson);

                string encodedResponse =
                    document.RootElement
                        .GetProperty("encoded")
                        .GetString() ?? "";

                ServerResponse? decodedResponse =
                    DecodeBase64<ServerResponse>(
                        encodedResponse
                    );

                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show(
                        decodedResponse?.message ??
                        "The server returned an error.",
                        "Server Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    return null;
                }

                return decodedResponse;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Could not connect to the marketplace server.\n\n" +
                    ex.Message,
                    "Connection Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );

                return null;
            }
        }

        // ==========================================
        // Local reaction storage
        // ==========================================

        private void LoadLocalReactions()
        {
            try
            {
                if (!File.Exists(reactionsFilePath))
                {
                    localReactions =
                        new List<LocalReaction>();

                    return;
                }

                string json =
                    File.ReadAllText(
                        reactionsFilePath
                    );

                localReactions =
                    JsonSerializer.Deserialize<
                        List<LocalReaction>
                    >(json)
                    ?? new List<LocalReaction>();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Could not load your saved reactions.\n\n" +
                    ex.Message,
                    "Reaction Storage Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                localReactions =
                    new List<LocalReaction>();
            }
        }

        private void SaveLocalReactions()
        {
            try
            {
                string? directoryPath =
                    Path.GetDirectoryName(
                        reactionsFilePath
                    );

                if (!string.IsNullOrWhiteSpace(
                    directoryPath))
                {
                    Directory.CreateDirectory(
                        directoryPath
                    );
                }

                JsonSerializerOptions options =
                    new JsonSerializerOptions
                    {
                        WriteIndented = true
                    };

                string json =
                    JsonSerializer.Serialize(
                        localReactions,
                        options
                    );

                File.WriteAllText(
                    reactionsFilePath,
                    json
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Could not save your reaction.\n\n" +
                    ex.Message,
                    "Reaction Storage Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }

        // ==========================================
        // Get local reaction
        // ==========================================

        private string? GetLocalReaction(
            int productId)
        {
            LocalReaction? reaction =
                localReactions.Find(
                    item => item.ProductId == productId
                );

            return reaction?.Reaction;
        }

        // ==========================================
        // Update reaction buttons
        // ==========================================

        private void UpdateReactionButtonStates()
        {
            if (selectedProductId == -1)
            {
                buttonPlusOne.Enabled = false;
                buttonMinusOne.Enabled = false;

                buttonPlusOne.BackColor =
                    SystemColors.Control;

                buttonMinusOne.BackColor =
                    SystemColors.Control;

                return;
            }

            buttonPlusOne.Enabled = true;
            buttonMinusOne.Enabled = true;

            string? reaction =
                GetLocalReaction(
                    selectedProductId
                );

            if (reaction == "like")
            {
                buttonPlusOne.BackColor =
                    Color.LightGreen;

                buttonMinusOne.BackColor =
                    SystemColors.Control;
            }
            else if (reaction == "dislike")
            {
                buttonPlusOne.BackColor =
                    SystemColors.Control;

                buttonMinusOne.BackColor =
                    Color.LightCoral;
            }
            else
            {
                buttonPlusOne.BackColor =
                    SystemColors.Control;

                buttonMinusOne.BackColor =
                    SystemColors.Control;
            }
        }

        // ==========================================
        // Authentication
        // ==========================================

        private async void buttonUseEncryptionCode_Click(
            object? sender,
            EventArgs e)
        {
            string encryptionCode =
                textBoxEnterEncryptionCode.Text.Trim();

            if (string.IsNullOrWhiteSpace(
                encryptionCode))
            {
                MessageBox.Show(
                    "Enter an encryption/access code first.",
                    "Missing Code",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            buttonUseEncryptionCode.Enabled = false;

            try
            {
                ServerResponse? response =
                    await SendBase64RequestAsync(
                        "/api/authenticate",
                        new
                        {
                            code = encryptionCode
                        }
                    );

                if (response == null)
                {
                    return;
                }

                if (!response.success)
                {
                    MessageBox.Show(
                        response.message,
                        "Authentication Failed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    return;
                }

                isAuthenticated = true;

                MessageBox.Show(
                    "Authentication successful.",
                    "Connected",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                buttonLoadProducts.Enabled = true;

                // Automatically display products.
                LoadProductsIntoTreeView(
                    response.products
                );
            }
            finally
            {
                buttonUseEncryptionCode.Enabled = true;
            }
        }

        // ==========================================
        // Load products
        // ==========================================

        private async void buttonLoadProducts_Click(
            object? sender,
            EventArgs e)
        {
            if (!isAuthenticated)
            {
                MessageBox.Show(
                    "Authenticate first."
                );

                return;
            }

            buttonLoadProducts.Enabled = false;

            try
            {
                ServerResponse? response =
                    await SendBase64RequestAsync(
                        "/api/products",
                        new
                        {
                            authenticated = true
                        }
                    );

                if (response?.success == true)
                {
                    LoadProductsIntoTreeView(
                        response.products
                    );
                }
            }
            finally
            {
                buttonLoadProducts.Enabled = true;
            }
        }

        private void LoadProductsIntoTreeView(
            List<Product>? receivedProducts)
        {
            if (receivedProducts == null)
            {
                return;
            }

            products.Clear();

            products.AddRange(
                receivedProducts
            );

            listBoxProducts.Nodes.Clear();

            Dictionary<string, TreeNode>
                categoryNodes =
                    new Dictionary<string, TreeNode>();

            foreach (Product product in products)
            {
                if (!categoryNodes.ContainsKey(
                    product.category))
                {
                    TreeNode categoryNode =
                        new TreeNode(
                            product.category
                        );

                    categoryNodes.Add(
                        product.category,
                        categoryNode
                    );

                    listBoxProducts.Nodes.Add(
                        categoryNode
                    );
                }

                TreeNode productNode =
                    new TreeNode(
                        product.title
                    );

                productNode.Tag =
                    product.id;

                categoryNodes[
                    product.category
                ].Nodes.Add(
                    productNode
                );
            }

            listBoxProducts.ExpandAll();

            // Reset selected product state.
            selectedProductId = -1;
            selectedProductImageUrl = "";

            textReadProductTitle.Text = "";

            imageBoxLoadImage.Image?.Dispose();
            imageBoxLoadImage.Image = null;

            buttonLoadImage.Enabled = false;
            buttonPlusOne.Enabled = false;
            buttonMinusOne.Enabled = false;
            buttonLogPurchase.Enabled = false;

            UpdateReactionButtonStates();
        }

        // ==========================================
        // Product selection
        // ==========================================

        private void listBoxProducts_AfterSelect(
            object? sender,
            TreeViewEventArgs e)
        {
            // Category nodes do not contain a product ID.
            if (e.Node.Tag is not int productId)
            {
                return;
            }

            Product? product =
                products.Find(
                    item => item.id == productId
                );

            if (product == null)
            {
                return;
            }

            selectedProductId =
                product.id;

            selectedProductImageUrl =
                product.image ?? "";

            textReadProductTitle.Text =
                product.title;

            textBoxQuantity.Text = "1";

            buttonLoadImage.Enabled =
                !string.IsNullOrWhiteSpace(
                    product.image
                );

            buttonPlusOne.Enabled = true;
            buttonMinusOne.Enabled = true;
            buttonLogPurchase.Enabled = true;

            imageBoxLoadImage.Image?.Dispose();
            imageBoxLoadImage.Image = null;

            // Restore the locally saved reaction
            // for this product.
            UpdateReactionButtonStates();
        }

        // ==========================================
        // Load image
        // ==========================================

        private async void buttonLoadImage_Click(
            object? sender,
            EventArgs e)
        {
            if (selectedProductId == -1)
            {
                MessageBox.Show(
                    "Select a product first."
                );

                return;
            }

            if (string.IsNullOrWhiteSpace(
                selectedProductImageUrl))
            {
                MessageBox.Show(
                    "This product has no image."
                );

                return;
            }

            buttonLoadImage.Enabled = false;

            try
            {
                byte[] imageBytes =
                    await httpClient.GetByteArrayAsync(
                        selectedProductImageUrl
                    );

                using MemoryStream stream =
                    new MemoryStream(
                        imageBytes
                    );

                using Image downloadedImage =
                    Image.FromStream(
                        stream
                    );

                imageBoxLoadImage.Image?.Dispose();

                imageBoxLoadImage.Image =
                    new Bitmap(
                        downloadedImage
                    );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Could not load image.\n\n" +
                    ex.Message,
                    "Image Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
            finally
            {
                buttonLoadImage.Enabled = true;
            }
        }

        // ==========================================
        // Like
        // ==========================================

        private async void buttonPlusOne_Click(
            object? sender,
            EventArgs e)
        {
            await SetReactionAsync(
                "like"
            );
        }

        // ==========================================
        // Dislike
        // ==========================================

        private async void buttonMinusOne_Click(
            object? sender,
            EventArgs e)
        {
            await SetReactionAsync(
                "dislike"
            );
        }

        // ==========================================
        // Set local reaction
        // ==========================================

        private async Task SetReactionAsync(
            string reaction)
        {
            if (!isAuthenticated)
            {
                MessageBox.Show(
                    "Authenticate first."
                );

                return;
            }

            if (selectedProductId == -1)
            {
                MessageBox.Show(
                    "Select a product first."
                );

                return;
            }

            if (reaction != "like" &&
                reaction != "dislike")
            {
                return;
            }

            LocalReaction? existingReaction =
                localReactions.Find(
                    item =>
                        item.ProductId ==
                        selectedProductId
                );

            string? finalReaction;

            // ======================================
            // Same reaction clicked again
            // ======================================

            if (existingReaction != null &&
                existingReaction.Reaction ==
                reaction)
            {
                // Remove the reaction.
                localReactions.Remove(
                    existingReaction
                );

                finalReaction = null;
            }

            // ======================================
            // Different reaction clicked
            // ======================================

            else if (existingReaction != null)
            {
                // Change Like -> Dislike
                // or Dislike -> Like.
                existingReaction.Reaction =
                    reaction;

                finalReaction = reaction;
            }

            // ======================================
            // No previous reaction
            // ======================================

            else
            {
                LocalReaction newReaction =
                    new LocalReaction
                    {
                        ProductId =
                            selectedProductId,

                        Reaction =
                            reaction
                    };

                localReactions.Add(
                    newReaction
                );

                finalReaction = reaction;
            }

            // Save immediately.
            SaveLocalReactions();

            // Update buttons immediately.
            UpdateReactionButtonStates();

            // ======================================
            // Send change to server
            // ======================================

            try
            {
                ServerResponse? response =
                    await SendBase64RequestAsync(
                        "/api/reaction",
                        new
                        {
                            authenticated = true,

                            productId =
                                selectedProductId,

                            reaction =
                                finalReaction
                        }
                    );

                if (response?.success == true)
                {
                    // Refresh the product's server counts.
                    Product? product =
                        products.Find(
                            item =>
                                item.id ==
                                selectedProductId
                        );

                    if (product != null)
                    {
                        product.likes =
                            response.likes;

                        product.dislikes =
                            response.dislikes;
                    }
                }
            }
            catch
            {
                // The local reaction has already been
                // saved, so it remains available even
                // if the server is temporarily unavailable.
            }
        }

        // ==========================================
        // Log purchase
        // ==========================================

        private async void buttonLogPurchase_Click(
            object? sender,
            EventArgs e)
        {
            if (!isAuthenticated)
            {
                MessageBox.Show(
                    "Authenticate first."
                );

                return;
            }

            if (selectedProductId == -1)
            {
                MessageBox.Show(
                    "Select a product first."
                );

                return;
            }

            if (!int.TryParse(
                textBoxQuantity.Text.Trim(),
                out int quantity) ||
                quantity <= 0)
            {
                MessageBox.Show(
                    "Enter a valid quantity greater than zero.",
                    "Invalid Quantity",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            string information =
                textBoxEnterInformation.Text.Trim();

            Product? product =
                products.Find(
                    item =>
                        item.id ==
                        selectedProductId
                );

            if (product == null)
            {
                MessageBox.Show(
                    "Product not found."
                );

                return;
            }

            if (quantity > product.stock)
            {
                MessageBox.Show(
                    $"Only {product.stock} item(s) are available.",
                    "Insufficient Stock",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            decimal estimatedTotal =
                product.price * quantity;

            DialogResult confirmation =
                MessageBox.Show(
                    $"Product: {product.title}\n" +
                    $"Quantity: {quantity}\n" +
                    $"Estimated total: ${estimatedTotal:F2}\n\n" +
                    "Log this purchase?",
                    "Confirm Purchase",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

            if (confirmation !=
                DialogResult.Yes)
            {
                return;
            }

            buttonLogPurchase.Enabled = false;

            try
            {
                ServerResponse? response =
                    await SendBase64RequestAsync(
                        "/api/purchase",
                        new
                        {
                            authenticated = true,

                            productId =
                                selectedProductId,

                            quantity =
                                quantity,

                            information =
                                information
                        }
                    );

                if (response?.success == true)
                {
                    MessageBox.Show(
                        $"Purchase logged successfully!\n\n" +
                        $"Purchase ID: {response.purchaseId}\n" +
                        $"Total: ${response.total:F2}\n" +
                        $"Remaining stock: {response.remainingStock}",
                        "Purchase Logged",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    await RefreshProductsAsync();
                }
            }
            finally
            {
                buttonLogPurchase.Enabled = true;
            }
        }

        // ==========================================
        // Refresh products
        // ==========================================

        private async Task RefreshProductsAsync()
        {
            ServerResponse? response =
                await SendBase64RequestAsync(
                    "/api/products",
                    new
                    {
                        authenticated = true
                    }
                );

            if (response?.success == true)
            {
                LoadProductsIntoTreeView(
                    response.products
                );
            }
        }

        // ==========================================
        // Cleanup
        // ==========================================

        protected override void OnFormClosed(
            FormClosedEventArgs e)
        {
            imageBoxLoadImage.Image?.Dispose();

            httpClient.Dispose();

            base.OnFormClosed(e);
        }
    }
}