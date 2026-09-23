using System.Net;
using System.Net.Http.Json;
using ECommerce.Api.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace ECommerce.Tests;

/// <summary>
/// Tests d'intégration de la route GET /products/{id:int} via WebApplicationFactory.
/// Chaque test crée sa propre factory pour isoler les données produits en mémoire.
/// </summary>
public sealed class ProductsIntegrationTests : IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public ProductsIntegrationTests()
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
    public async Task GetProduct_Existant_Retourne200AvecLeProduitAttendu()
    {
        // Act
        var response = await _client.GetAsync("/products/1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var product = await response.Content.ReadFromJsonAsync<Product>();
        product.Should().NotBeNull();
        product!.Id.Should().Be(1);
        product.Name.Should().Be("Clavier mécanique");
        product.Price.Should().Be(79.90m);
        product.Stock.Should().Be(25);
    }

    [Fact]
    public async Task GetProduct_IdInconnu_Retourne404()
    {
        // Act
        var response = await _client.GetAsync("/products/999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [InlineData(1, "Clavier mécanique", 79.90, 25)]
    [InlineData(2, "Souris sans fil", 29.90, 40)]
    [InlineData(3, "Écran 27\" 4K", 349.00, 10)]
    [InlineData(4, "Casque audio", 59.90, 15)]
    public async Task GetProduct_TousLesProduitsEnMemoire_Retourne200AvecLeBonProduit(
        int id, string name, decimal price, int stock)
    {
        // Act
        var response = await _client.GetAsync($"/products/{id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var product = await response.Content.ReadFromJsonAsync<Product>();
        product.Should().NotBeNull();
        product!.Id.Should().Be(id);
        product.Name.Should().Be(name);
        product.Price.Should().Be(price);
        product.Stock.Should().Be(stock);
    }

    [Fact]
    public async Task GetProduct_NonEntier_Retourne404_EchecDeRoutage()
    {
        // Act — la contrainte de route {id:int} refuse un segment non entier :
        // aucune route ne correspond, donc 404 (échec de routage, pas NotFound métier).
        var response = await _client.GetAsync("/products/abc");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetProduct_IdNegatif_Retourne404()
    {
        // Act
        var response = await _client.GetAsync("/products/-1");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
