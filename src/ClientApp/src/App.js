import React from "react";
import { Routes, Route } from "react-router";
import { Layout } from "./components/Layout";
import { Home } from "./components/Home";
import { Detail } from "./components/Detail";
import { ThemeProvider, createTheme } from "@mui/material/styles";
import CssBaseline from "@mui/material/CssBaseline";
import "@fontsource/roboto/300.css";
import "@fontsource/roboto/400.css";
import "@fontsource/roboto/500.css";
import "@fontsource/roboto/700.css";

import "./custom.css";

const theme = createTheme({
  palette: {
    primary: {
      main: "#007CC3",
      contrastText: "#fff",
    },
  },
});

export default function App() {
  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <Layout>
        <Routes>
          <Route exact path="/" element={<Home />} />
          <Route path="/detail/:md5/:name" element={<Detail />} />
        </Routes>
      </Layout>
    </ThemeProvider>
  );
}
