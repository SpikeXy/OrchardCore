LocalizationPartDriver -> LocalizationPart_Edit shape -> LocalizationPart.Edit.cshtml


List all available cultures (SupportedCultures) => "Create Localization" button for each culture
Render the current culture of the item, and also other localizations for the current item

New AdminController.cs
-> POST>Create
    -> IContentLocalizationService.Localize()

In Orchard.ContentLocalization.Abstractions
Create IContentLocalizationService
- Localize(ContentItem)
    -> Clone the content 
    -> Localizing() Then Localized() on the new content item
    -> Set the localization set and the culture

Create IContentLocalizationEventHandler
- Localizing()
- Localized()

In Orchard.ContentLocalization.Core
Implement IContentLocalizationService
