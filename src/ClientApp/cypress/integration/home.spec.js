describe("Landing page tests", () => {
  it("Displays welcome message in german", () => {
    const germanWelcome = "Willkommen";

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

    cy.contains(germanWelcome);
  });

  it("Displays welcome message in french", () => {
    const frenchWelcome = "Bienvenue";

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

    cy.contains(frenchWelcome);
  });

  it("Displays welcome message in italia", () => {
    const italianWelcome = "Benvenuto";

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
    cy.contains(italianWelcome);
  });
});
