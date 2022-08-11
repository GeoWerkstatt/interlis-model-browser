import React from "react";
import { Button, Stack, Typography } from "@mui/material";
import FilterAltIcon from "@mui/icons-material/FilterAlt";
import { useTranslation } from "react-i18next";

export function Results(props) {
  const { numberOfResults } = props;
  const { t } = useTranslation("common");

  return (
    <Stack direction="row" justifyContent="space-between" alignItems="flex-end" spacing={2}>
      <Typography variant="h5" mt={6} ml={1}>
        {numberOfResults + " " + t("models-found", { count: numberOfResults })}
      </Typography>
      <Button variant="outlined" startIcon={<FilterAltIcon />}>
        {t("filter")}
      </Button>
    </Stack>
  );
}
