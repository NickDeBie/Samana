# Samana



Ontstaan van het project
------------------------
Dit project is gestart om mijn moeder te helpen bij haar vrijwilligerswerk bij Samana. Zij doet vrijwilligerswerk voor gepensioneerde mensen. Deze mensen bezoeken alsook organiseren van evenementen en bijeenkomsten. Voor deze te organiseren heeft ze de persoonlijke gegevens nodig die ze bewaarde in verschillende excel bestanden. Wat dus betekent dat wanneer er iets wijzigt in die informatie (wat vrij frequent gebeurd) moest ze al die verschillende bestanden gaan aanpassen en dus heel tijdconsumerend was. Dit wou ik dan voor haar gemakkelijker maken en al die informatie centraliseren om dan zo de nodige excel bestanden te genereren. 



Technologiekeuze
----------------
Ik heb gekozen om webapplicatie in ASP te maken met MVC. Dit om de simpele reden dat ik hierin ervaring wou opdoen. In zowel C# als met HTML, CSS, Javascript/Jquery. De database is opgesteld met behulp van entity framework in mySQL. 



Beschrijving van de applicatie
------------------------------
De database bevat al de gegevens van de leden. Er zijn 3 verschillende leden. Kernleden, medewerkers en gewoon leden. De kernleden zijn mentor van verschillende medewerkers en gewoon leden. Deze worden op het startscherm getoond en afhankelijk van de lidsoort wordt de belangrijkste informatie getoond. Zoals noodpersonen, familie/vrienden die men kan contacteren in geval van nood. Er wordt ook wat algemene info getoond over de leden in het algemeen. Deze info is niet noodzakelijk maar zijn er eerder als leuke weetjes.

Op dit scherm kan ook een nieuw lid aangemaakt worden of de bestaande leden kunnen worden aangepast. Ook heb ik een paar zoekfuncties toegevoegd om leden te zoeken op lidsoort of op voornaam/familienaam.

De kernleden krijgen een extra knop om daarvan een excel bestand te genereren en vervolgens te downloaden. In dit excel bestand worden 2 worksheets aangemaakt. 
De eerste sheet bevat het kernlid en zijn/haar 'beschermelingen' en hun adres. Deze lijst is bedoeld voor het kernlid om zijn beschermelingen te bezoeken. Er staat dan ook bij om hoeveel beschermelingen het gaat en om hoeveel te bezoeken huizen.
In de tweede sheet staan de verjaardagen van elke beschermeling. Deze worden tevens gesorteerd op eerstkomende verjaardag. 

