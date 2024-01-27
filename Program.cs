using Newtonsoft.Json;

// Get the directories for the two required files
//  to read from and write to.
var productsDir = Path.Combine(Directory.GetCurrentDirectory(), "assests", "products.json");
var ordersDir = Path.Combine(Directory.GetCurrentDirectory(), "assests", "orders.json");

// Heading message
Console.WriteLine("Welcome to Simple Finances System");
// Continue to loop through options until the user selects quit
while (true)
{
    // Display available options
    Console.WriteLine($"{Environment.NewLine}What would you like to do?{Environment.NewLine}1. Create a purchase order{Environment.NewLine}2. View all orders{Environment.NewLine}3. Create a new product{Environment.NewLine}4. View all products{Environment.NewLine}5. Exit");
    // Store selected option
    var option = Console.ReadLine();
    // Determine what to do based on selected option
    switch (option)
    {
        // If the user selects 1
        case "1":
            // Get user's name
            Console.Write($"{Environment.NewLine}Enter your name: ");
            var orderName = Console.ReadLine();

            // Display available products
            Console.WriteLine($"{Environment.NewLine}Products:");
            displayProducts(GetProducts(), false);

            // Store exisitng products
            List<ProductData> exisitingProducts = (List<ProductData>)GetProducts();
            // Create a new list for the user's chosen products
            List<ProductData> products = [];
            while (true)
            {
                // Get the product number
                Console.Write($"{Environment.NewLine}Enter the item number you want to buy: ");
                var productNum = Console.ReadLine();

                // Get the quantity the user wants
                Console.Write("Enter the quantity you want: ");
                var qty = Convert.ToInt32(Console.ReadLine());

                // Create a product with the given info
                // This uses the product number given by the user to get the product info from the list of existing products 
                ProductData product = new(exisitingProducts[Convert.ToInt32(productNum) - 1].Name, exisitingProducts[Convert.ToInt32(productNum) - 1].Colour, exisitingProducts[Convert.ToInt32(productNum) - 1].Price, qty);

                // Add the product to the list of chosen products
                products.Add(product);

                // Does the user want to add another product?
                Console.WriteLine("Would you like to add another product (y/n)? ");
                // Break out of loop if not
                if (!Console.ReadLine()!.Equals("y", StringComparison.CurrentCultureIgnoreCase)) break;
            }
            // Create the product and write to file
            createOrder(orderName!, products);
            Console.WriteLine("Order created.");
            break;

        // If the user selects 2
        case "2":
            // Display exisitng orders from file
            Console.WriteLine($"{Environment.NewLine}Generating purchase orders...{Environment.NewLine}");
            displayOrders();
            break;

        // If the user selects 3
        case "3":
            // Get the product's name
            Console.Write($"{Environment.NewLine}Enter the item name: ");
            var productName = Console.ReadLine();

            // Get the product's colour
            Console.Write("Enter the item colour: ");
            var productColour = Console.ReadLine();

            // Get the product's price
            Console.Write("Enter the item price (xx,xx): ");
            var productPrice = Convert.ToDouble(Console.ReadLine());

            // Create a new product and write to file
            createProduct(productName!, productColour!, productPrice!, 0);
            Console.WriteLine("Product added.");
            break;

        // If the user selects 5
        case "4":
            // Display exisitng products from file
            Console.WriteLine($"{Environment.NewLine}Generating product list...{Environment.NewLine}");
            displayProducts(GetProducts(), false);
            break;

        // If the user enters anything else
        default:
            // Break out of the outer while loop
            // This will end the program
            return;
    }
}
/**
*   Returns an IEnumerable of products from the file at productsDir
*/
IEnumerable<ProductData> GetProducts()
{
    // Get file contents
    string itemsJson = File.ReadAllText(productsDir);
    // Converts it from JSON to a usable format
    IEnumerable<ProductData>? data = JsonConvert.DeserializeObject<IEnumerable<ProductData>>(itemsJson);
    // Return the data
    return data!;
}

/**
*   Return an IEnumerable of orders from the file at ordersDir
*/
IEnumerable<OrderData> GetOrders()
{
    // Get file contents
    string itemsJson = File.ReadAllText(ordersDir);
    // Converts it from JSON to a usable format
    IEnumerable<OrderData>? data = JsonConvert.DeserializeObject<IEnumerable<OrderData>>(itemsJson);
    // Return the data
    return data!;
}

/**
*   Creates a Product and writes to the file at productsDir
*/
void createProduct(string name, string colour, double price, int qty)
{
    // Get exisitng products
    List<ProductData> products = (List<ProductData>)GetProducts();

    // Add a new product from params
    products.Add(new ProductData(name, colour, price, qty));
    // Convert the list of products to JSON
    var json = JsonConvert.SerializeObject(products, Formatting.Indented);

    // Write the JSON of products to the file
    File.WriteAllText(productsDir, json);
}

/**
*   Creates an Order and writes to the file at ordersDir
*/
void createOrder(string name, IEnumerable<ProductData> products)
{
    // Get exisitng orders
    List<OrderData> orders = new(GetOrders());

    // Running total 
    double total = 0;
    // Loop through the products IEnum from params
    foreach (var product in products)
    {
        // Increase total based on product price and quantity
        total += product.Price * product.Qty;
    }
    // Add the new order with calculated total
    orders.Add(new OrderData(orders.Count > 0 ? orders[^1].OrderNumber + 1 : 1, name, DateTime.Now.ToString(), products, total));
    // Convert the list of orders to JSON
    var json = JsonConvert.SerializeObject(orders, Formatting.Indented);

    // Write the JSON of orders to the file
    File.WriteAllText(ordersDir, json);
}

/**
*   Displays all Products, from param products, to the Terminal
*   Params to for displaying quantity
*/
void displayProducts(IEnumerable<ProductData> products, bool qty)
{
    // Display header
    // Add "Qty' header if param qty is true
    Console.WriteLine($"No\tName\tColour\tPrice{(qty ? "\tQty" : "")}");
    // Running index of products
    int index = 1;
    // Loop through all products
    foreach (var product in products)
    {
        // Display product info
        // Display quantity if param qty is true
        Console.WriteLine($"{index}\t{product.Name}\t{product.Colour}\tR{product.Price}{(qty ? "\t" + product.Qty : "")}");
        // Increase index
        index++;
    }
}

/**
*   Displays all orders to the Terminal
*/
void displayOrders()
{
    // Loop through all orders
    foreach (var order in GetOrders())
    {
        // Display order number, customer name, and order date
        Console.WriteLine($"Order: order-{order.OrderNumber}{Environment.NewLine}Customer Name: {order.Name}{Environment.NewLine}Order Date: {order.Date}{Environment.NewLine}{Environment.NewLine}");
        // Display products from order
        displayProducts(order.Products, true);
        // Display total
        Console.WriteLine($"{Environment.NewLine}\t\tTotal: R{order.Total}{Environment.NewLine}---------------------------------{Environment.NewLine}");
    }
}

// Create a ProductData type
record ProductData(string Name, string Colour, double Price, int Qty);
// Create an OrderData Type
record OrderData(int OrderNumber, string Name, string Date, IEnumerable<ProductData> Products, double Total);