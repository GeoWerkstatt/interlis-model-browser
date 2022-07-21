import React from "react";
import { Routes, Route } from "react-router";
import { Layout } from "./components/Layout";
import { Home } from "./components/Home";
import CssBaseline from "@mui/material/CssBaseline";

import "@fontsource/roboto/300.css";
import "@fontsource/roboto/400.css";
import "@fontsource/roboto/500.css";
import "@fontsource/roboto/700.css";

import "./custom.css";

export default function App() {
  return (
    <React.Fragment>
      <CssBaseline />
      <Layout>
        <Routes>
          <Route exact path="/" element={<Home />} />
        </Routes>
      </Layout>
    </React.Fragment>
  );
}
