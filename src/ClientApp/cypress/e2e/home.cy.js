describe("Landing page tests", () => {
  it("Displays search message in german", () => {
    const germanSearch = "Suchen";

    cy.visit("/", {
      onBeforeLoad(win) {
        Object.defineProperty(win.navigator, "language", { value: "de-CH" });
        Object.defineProperty(win.navigator, "languages", { value: ["de"] });
        Object.defineProperty(win.navigator, "accept_languages", { value: ["de"] });
      },
      headers: {
        "Accept-Language": "de",
      },
    });

    cy.contains(germanSearch);
  });

  it("Displays search message in french", () => {
    const frenchSearch = "Recherche";

    cy.visit("/", {
      onBeforeLoad(win) {
        Object.defineProperty(win.navigator, "language", { value: "fr-CH" });
        Object.defineProperty(win.navigator, "languages", { value: ["fr"] });
        Object.defineProperty(win.navigator, "accept_languages", { value: ["fr"] });
      },
      headers: {
        "Accept-Language": "fr",
      },
    });

    cy.contains(frenchSearch);
  });

  it("Displays search message in italia", () => {
    const italianSearch = "Cerca";

    cy.visit("/", {
      onBeforeLoad(win) {
        Object.defineProperty(win.navigator, "language", { value: "it-CH" });
        Object.defineProperty(win.navigator, "languages", { value: ["it"] });
        Object.defineProperty(win.navigator, "accept_languages", { value: ["it"] });
      },
      headers: {
        "Accept-Language": "it",
      },
    });
    cy.contains(italianSearch);
  });

  it("Displays results after search", () => {
    cy.visit("/");
    cy.get("div[name=home]").should("not.contain", "Filter");
    cy.get("div[name=home]").should("not.contain", "Modelle gefunden");
    cy.get("input[name=searchInput]").should("be.visible").click().type("SearchString{enter}");
    cy.get("div[name=home]").should("contain", "Filter");
    cy.get("div[name=home]").should("contain", "Modelle gefunden");
  });
});
