using System.Net;
using System.Net.Http.Json;
using ECommerce.Api.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace ECommerce.Tests;

/// <summary>
/// Tests d'intégration du filtrage de la route GET /products par catégorie
/// (query param optionnel ?category=...) via WebApplicationFactory.
/// Chaque test crée sa propre factory pour isoler les données produits en mémoire.
/// </summary>
public sealed class ProductsFilterIntegrationTests : IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public ProductsFilterIntegrationTests()
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
    public async Task GetProducts_FiltreParCategorieInformatique_Retourne200AvecLes3ProduitsInformatique()
    {
        // Act
        var response = await _client.GetAsync("/products?category=informatique");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var products = await response.Content.ReadFromJsonAsync<List<Product>>();
        products.Should().NotBeNull().And.HaveCount(3);
        products!.Select(p => p.Name).Should().Equal(
            "Clavier mécanique", "Souris sans fil", "Écran 27\" 4K");
        products.Should().OnlyContain(p => p.Category == "informatique");
    }

    [Fact]
    public async Task GetProducts_FiltreParCategorieAudio_Retourne200AvecLeCasqueAudio()
    {
        // Act
        var response = await _client.GetAsync("/products?category=audio");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var products = await response.Content.ReadFromJsonAsync<List<Product>>();
        products.Should().NotBeNull().And.HaveCount(1);
        products![0].Name.Should().Be("Casque audio");
        products[0].Category.Should().Be("audio");
    }

    [Fact]
    public async Task GetProducts_SansParametreCategorie_Retourne200AvecLes4Produits()
    {
        // Act
        var response = await _client.GetAsync("/products");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var products = await response.Content.ReadFromJsonAsync<List<Product>>();
        products.Should().NotBeNull().And.HaveCount(4);
    }

    [Fact]
    public async Task GetProducts_CategorieInconnue_Retourne200AvecListeVide()
    {
        // Act
        var response = await _client.GetAsync("/products?category=inconnue");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var products = await response.Content.ReadFromJsonAsync<List<Product>>();
        products.Should().NotBeNull().And.BeEmpty();
    }
}