using FTELMarket;

public class FlakyService(HttpClient client)
{
    public void Sample()
    {
        // solution de gestion des HttpClient relative à la solution 1 du TP
        var baseAddress = client.BaseAddress;
    }

    public async Task<List<ClientRequest>> GetClientsAsync()
    {
        var result = new List<ClientRequest>();
        var page = 1;
        const int pageSize = 500;

        while (true)
        {
            var response = await client.GetFromJsonAsync<PageRequest<ClientRequest>>(
                $"/api/customers?page={page}&pageSize={pageSize}");

            if (response is null ||  response.Items.Count == 0) break;

            result.AddRange(response.Items);

            if (result.Count >= response.Total) break;

            page++;
        }

        return result;
    }

    public async Task<List<FlakyProductRequest>> GetProductsAsync()
    {
        var result = new List<FlakyProductRequest>();
        var page = 1;
        const int pageSize = 500;

        while (true)
        {
            var response = await client.GetFromJsonAsync<PageRequest<FlakyProductRequest>>(
                $"/api/products?page={page}&pageSize={pageSize}");

            if (response is null || response.Items.Count == 0) break;

            result.AddRange(response.Items);

            if (result.Count >= response.Total) break;

            page++;
        }

        return result;
    }

    public async Task<List<OrderRequest>> GetOrdersAsync()
    {
        var result = new List<OrderRequest>();
        var page = 1;
        const int pageSize = 500;

        while (true)
        {
            Console.WriteLine($"Requête commandes page {page}");
            var response = await client.GetFromJsonAsync<PageRequest<OrderRequest>>(
                $"/api/orders?page={page}&pageSize={pageSize}");

            if (response is null || response.Items.Count == 0) break;

            result.AddRange(response.Items);

            if (result.Count >= response.Total) break;

            page++;
        }

        return result;
    }
}