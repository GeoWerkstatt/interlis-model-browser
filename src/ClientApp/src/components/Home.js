import React from "react";
import Button from "@mui/material/Button";
import { useTranslation } from "react-i18next";

export function Home() {
  const { t } = useTranslation("common");
  return (
    <div>
      <h1>{t("welcome")}</h1>
      <Button>INTERLIS Model Repo Browser</Button>
    </div>
  );
}
