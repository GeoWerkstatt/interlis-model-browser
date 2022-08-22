import React, { useState, useEffect } from "react";
import { useSearchParams } from "react-router-dom";
import { Autocomplete, Box, Button, CircularProgress, IconButton, TextField, Tooltip, Stack } from "@mui/material";
import { useTranslation } from "react-i18next";
import SearchIcon from "@mui/icons-material/Search";
import ClearIcon from "@mui/icons-material/Clear";
import { useForm } from "react-hook-form";
import { Results } from "./Results";

export function Home() {
  const { register, handleSubmit, watch, reset } = useForm();
  const [searchParams, setSearchParams] = useSearchParams();
  const [models, setModels] = useState(null);
  const [inputValue, setInputValue] = useState(searchParams.get("query") || "");
  const [repositoryTree, setRepositoryTree] = useState();
  const [searchOptions, setSearchOptions] = useState([]);
  const [loading, setLoading] = useState();

  const { t } = useTranslation("common");

  async function search(searchString) {
    setSearchParams(
      {
        query: searchString,
      },
      { replace: true }
    );
    setLoading(true);
    const response = await fetch("/search?query=" + searchString);
    if (response.ok) {
      if (response.status === 204 /* No Content */) {
        setModels([]);
        setLoading(false);
      } else {
        const repositoryTree = await response.json();
        setRepositoryTree(repositoryTree);
        setModels(getAllModels(repositoryTree));
        setLoading(false);
      }
    } else {
      setModels([]);
      setLoading(false);
    }
  }

  async function getSearchOptions(searchString) {
    if (searchString.length < 3) {
      setSearchOptions([]);
    } else {
      const response = await fetch("/search/suggest/" + searchString);
      if (response.ok && response.status !== 204 /* No Content */) {
        const suggestions = await response.json();
        setSearchOptions([...new Set(suggestions)]);
      }
    }
  }

  function getAllModels(repository) {
    const modelRepository = repository.title + " [" + repository.name + "]";

    return [
      ...repository.models.map((m) => {
        m.modelRepository = modelRepository;
        return m;
      }),
      ...repository.subsidiarySites.flatMap((r) => getAllModels(r)),
    ];
  }

  const onSubmit = (data) => search(data.searchInput);

  const clear = () => {
    reset({ searchInput: "" });
    setInputValue("");
    setSearchOptions([]);
    setModels(null);
  };

  // If component is first loaded with search params present in URL the search should immediately be executed.
  useEffect(() => {
    if (inputValue !== "") {
      search(inputValue);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  return (
    <Box name="home">
      <form onSubmit={handleSubmit(onSubmit)} name="search-form">
        <Stack mt={20} direction="row" justifyContent="space-between" alignItems="strech">
          <Autocomplete
            freeSolo
            inputValue={inputValue}
            onInputChange={(event, newInputValue) => {
              setInputValue(newInputValue);
              getSearchOptions(newInputValue);
            }}
            options={searchOptions}
            fullWidth
            renderInput={(params) => (
              <TextField
                {...params}
                margin="normal"
                fullWidth
                label={t("search-instructions")}
                variant="outlined"
                {...register("searchInput", { required: true })}
                InputProps={{
                  ...params.InputProps,
                  style: {
                    padding: 10,
                  },
                  startAdornment: <SearchIcon color="disabled" />,
                  endAdornment: (
                    <Tooltip title={t("reset-search")}>
                      <IconButton
                        sx={{ visibility: watch("searchInput") !== "" ? "visible" : "hidden" }}
                        onClick={clear}
                      >
                        <ClearIcon />
                      </IconButton>
                    </Tooltip>
                  ),
                }}
              />
            )}
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
      {!models && loading && (
        <Box mt={10}>
          <CircularProgress />
        </Box>
      )}
      {models !== null && (
        <Results models={models} repositoryTree={repositoryTree} searchParams={searchParams}></Results>
      )}
    </Box>
  );
}
