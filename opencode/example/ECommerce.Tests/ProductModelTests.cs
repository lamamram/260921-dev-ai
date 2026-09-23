using ECommerce.Api.Models;
using FluentAssertions;
using Xunit;

namespace ECommerce.Tests;

/// <summary>
/// Tests unitaires du modèle Product, en particulier l'ajout de la propriété Category
/// (record positionnel à 5 paramètres : Id, Name, Price, Stock, Category).
/// </summary>
public sealed class ProductModelTests
{
    [Fact]
    public void Product_ConstructeurPositionnel_AvecCategorie_ConserveToutesLesValeurs()
    {
        // Act
        var product = new Product(1, "Clavier mécanique", 79.90m, 25, "informatique");

        // Assert
        product.Id.Should().Be(1);
        product.Name.Should().Be("Clavier mécanique");
        product.Price.Should().Be(79.90m);
        product.Stock.Should().Be(25);
        product.Category.Should().Be("informatique");
    }

    [Fact]
    public void Product_DeconstructionPositionnelle_RetourneLaCategorie()
    {
        // Arrange
        var product = new Product(4, "Casque audio", 59.90m, 15, "audio");

        // Act
        var (id, name, price, stock, category) = product;

        // Assert
        id.Should().Be(4);
        name.Should().Be("Casque audio");
        price.Should().Be(59.90m);
        stock.Should().Be(15);
        category.Should().Be("audio");
    }
}