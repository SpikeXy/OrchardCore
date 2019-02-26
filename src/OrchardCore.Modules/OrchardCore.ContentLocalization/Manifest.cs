using OrchardCore.Modules.Manifest;

[assembly: Module(
    Name = "Content Localization",
    Author = "The Orchard Team",
    Website = "http://orchardproject.net",
    Version = "2.0.0",
    Description = "The alias module enables content items to have custom logical identifier.",
    Dependencies = new [] { "OrchardCore.Content" },
    Category = "Content Management"
)]
