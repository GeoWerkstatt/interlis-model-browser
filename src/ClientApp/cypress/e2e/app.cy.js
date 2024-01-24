describe("General app tests", () => {
  it("Displays the application version", () => {
    const expectedVersion = "457.31.0.4651";
    cy.intercept("/version", expectedVersion);

    cy.visit("/");
    cy.contains("Version: " + expectedVersion);
  });
});
