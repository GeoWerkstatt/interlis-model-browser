import React, { useState } from "react";
import { Box, Button, IconButton, TextField, Tooltip, Stack } from "@mui/material";
import { useTranslation } from "react-i18next";
import SearchIcon from "@mui/icons-material/Search";
import ClearIcon from "@mui/icons-material/Clear";
import { useForm } from "react-hook-form";
import { Results } from "./Results";

export function Home() {
  const { register, handleSubmit, watch, reset } = useForm();
  const [models, setModels] = useState(null);

  async function search(searchString) {
    const response = await fetch("/search?query=" + searchString);
    if (response.ok) {
      setModels(await response.json());
    } else {
      setModels([]);
    }
  }

  const onSubmit = (data) => search(data.searchInput);
  const clear = () => {
    reset({ searchInput: "" });
    setModels(null);
  };

  const { t } = useTranslation("common");

  return (
    <Box name="home">
      <form onSubmit={handleSubmit(onSubmit)} name="search-form">
        <Stack mt={20} direction="row" justifyContent="space-between" alignItems="strech">
          <TextField
            margin="normal"
            fullWidth
            label={t("search-instructions")}
            variant="outlined"
            {...register("searchInput", { required: true })}
            InputProps={{
              startAdornment: <SearchIcon color="disabled" />,
              endAdornment: (
                <Tooltip title={t("reset-search")}>
                  <IconButton sx={{ visibility: watch("searchInput") !== "" ? "visible" : "hidden" }} onClick={clear}>
                    <ClearIcon />
                  </IconButton>
                </Tooltip>
              ),
            }}
          />
          <Button
            sx={{
              marginLeft: "-5px",
              marginBottom: "8px",
              marginTop: "16px",
              borderTopLeftRadius: 0,
              borderBottomLeftRadius: 0,
            }}
            type="submit"
            variant="contained"
            aria-label="search"
            color="primary"
          >
            {t("search")}
          </Button>
        </Stack>
      </form>
      {models !== null && <Results models={models}></Results>}
    </Box>
  );
}
