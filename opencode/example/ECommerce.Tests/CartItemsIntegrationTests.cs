using System.Net;
using System.Net.Http.Json;
using ECommerce.Api.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace ECommerce.Tests;

/// <summary>
/// Tests d'intégration de la route POST /cart/items via WebApplicationFactory.
/// Chaque test crée sa propre factory pour isoler le panier en mémoire.
/// </summary>
public sealed class CartItemsIntegrationTests : IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public CartItemsIntegrationTests()
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    [Fact]
    public async Task PostCartItems_AvecProduitValide_Retourne200EtPanier()
    {
        // Arrange
        var item = new CartItem(ProductId: 1, Quantity: 2);

        // Act
        var response = await _client.PostAsJsonAsync("/cart/items", item);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var cart = await response.Content.ReadFromJsonAsync<List<CartItem>>();
        cart.Should().NotBeNull().And.HaveCount(1);
        cart![0].ProductId.Should().Be(1);
        cart[0].Quantity.Should().Be(2);
    }

    [Fact]
    public async Task PostCartItems_ProduitInconnu_Retourne404()
    {
        // Arrange
        var item = new CartItem(ProductId: 999, Quantity: 1);

        // Act
        var response = await _client.PostAsJsonAsync("/cart/items", item);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("Produit inconnu");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-3)]
    public async Task PostCartItems_QuantiteInvalide_Retourne400(int quantity)
    {
        // Arrange
        var item = new CartItem(ProductId: 1, Quantity: quantity);

        // Act
        var response = await _client.PostAsJsonAsync("/cart/items", item);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("Quantité invalide");
    }

    [Fact]
    public async Task PostCartItems_DeuxFoisMemeProduit_CumuleLesQuantites()
    {
        // Arrange
        var premierAjout = new CartItem(ProductId: 2, Quantity: 1);
        var secondAjout = new CartItem(ProductId: 2, Quantity: 3);

        // Act
        await _client.PostAsJsonAsync("/cart/items", premierAjout);
        var response = await _client.PostAsJsonAsync("/cart/items", secondAjout);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var cart = await response.Content.ReadFromJsonAsync<List<CartItem>>();
        cart.Should().NotBeNull().And.HaveCount(1);
        cart![0].ProductId.Should().Be(2);
        cart[0].Quantity.Should().Be(4);
    }

    [Fact]
    public async Task PostCartItems_PuisGetCart_RefleteLePanier()
    {
        // Arrange
        var item = new CartItem(ProductId: 3, Quantity: 5);

        // Act
        await _client.PostAsJsonAsync("/cart/items", item);
        var cartResponse = await _client.GetAsync("/cart");

        // Assert
        cartResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var cart = await cartResponse.Content.ReadFromJsonAsync<List<CartItem>>();
        cart.Should().NotBeNull().And.HaveCount(1);
        cart![0].ProductId.Should().Be(3);
        cart[0].Quantity.Should().Be(5);
    }
}
