import React from "react";
import { Routes, Route } from "react-router";
import { Layout } from "./components/Layout";
import { Home } from "./components/Home";
import { Detail } from "./components/Detail";
import { ThemeProvider, createTheme } from "@mui/material/styles";
import { LocalizationProvider } from "@mui/x-date-pickers";
import { AdapterDayjs } from "@mui/x-date-pickers/AdapterDayjs";
import "dayjs/locale/de-ch";
import "dayjs/locale/fr-ch";
import "dayjs/locale/it-ch";
import { useTranslation } from "react-i18next";
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
  const { i18n } = useTranslation();

  return (
    <ThemeProvider theme={theme}>
      <LocalizationProvider dateAdapter={AdapterDayjs} adapterLocale={i18n.language + "-ch"}>
        <CssBaseline />
        <Layout>
          <Routes>
            <Route exact path="/" element={<Home />} />
            <Route path="/detail/:md5/:name" element={<Detail />} />
            <Route path="*" element={<Home />} />
          </Routes>
        </Layout>
      </LocalizationProvider>
    </ThemeProvider>
  );
}
