import React, { useState, useEffect } from "react";
import { useSearchParams } from "react-router-dom";
import { Autocomplete, Box, Button, CircularProgress, IconButton, TextField, Tooltip, Stack } from "@mui/material";
import { useTranslation } from "react-i18next";
import SearchIcon from "@mui/icons-material/Search";
import ClearIcon from "@mui/icons-material/Clear";
import { useForm } from "react-hook-form";
import { Results } from "./Results";
import { FilterValues } from "./FilterValues";

export function Home() {
  const { register, handleSubmit, watch, reset } = useForm();
  const [searchParams, setSearchParams] = useSearchParams();
  const [models, setModels] = useState(null);
  const [inputValue, setInputValue] = useState(searchParams.get("query") || "");
  const [hideFilter, setHideFilter] = useState(false);
  const [schemaLanguages, setSchemaLanguages] = useState([]);
  const [issuers, setIssuers] = useState([]);
  const [repositoryNames, setRepositoryNames] = useState([]);
  const [dependsOnModels, setDependsOnModels] = useState([]);
  const [repositoryTree, setRepositoryTree] = useState();
  const [searchOptions, setSearchOptions] = useState([]);
  const [loading, setLoading] = useState();
  const [searchUrl, setSearchUrl] = useState();

  const { t } = useTranslation("common");

  const getSearchUrl = () => {
    var url = new URL(window.location);
    issuers.forEach((issuer) => {
      url.searchParams.append(FilterValues.Issuers, issuer);
    });
    repositoryNames.forEach((name) => {
      url.searchParams.append(FilterValues.RepositoryNames, name);
    });
    schemaLanguages.forEach((language) => {
      url.searchParams.append(FilterValues.SchemaLanguages, language);
    });
    dependsOnModels.forEach((model) => {
      url.searchParams.append(FilterValues.DependsOnModels, model);
    });
    if (searchParams.get("hideFilter") === undefined) {
      url.searchParams.append("hideFilter", !!hideFilter);
    }
    setSearchUrl(url);
    return url;
  };

  const sortRepositoryTree = (nodes) => {
    nodes.sort((a, b) => a.name.localeCompare(b.name));
    nodes.forEach(function (node) {
      if (node.subsidiarySites.length > 0) {
        sortRepositoryTree(node.subsidiarySites);
      }
    });
  };

  async function search(searchString) {
    const url = getSearchUrl();
    setSearchParams(
      {
        query: searchString,
      },
      { replace: true }
    );
    setLoading(true);
    url.searchParams.set("query", searchString);

    const response = await fetch("/search" + url.search);
    if (response.ok) {
      if (response.status === 204 /* No Content */) {
        setModels([]);
        setLoading(false);
      } else {
        const repositoryTree = await response.json();
        sortRepositoryTree(repositoryTree.subsidiarySites);
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
    const url = getSearchUrl();

    if (searchString.length < 3) {
      setSearchOptions([]);
    } else {
      url.searchParams.append("query", searchString);
      const response = await fetch("/search/suggest/" + url.search);
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
  // On first load the hideFilter state should be set for all following requests.
  useEffect(() => {
    setHideFilter(searchParams.get("hideFilter"));
    !!searchParams.get(FilterValues.SchemaLanguages)
      ? setSchemaLanguages(searchParams.getAll(FilterValues.SchemaLanguages))
      : setSchemaLanguages([]);
    !!searchParams.get(FilterValues.Issuers) ? setIssuers(searchParams.getAll(FilterValues.Issuers)) : setIssuers([]);
    !!searchParams.get(FilterValues.RepositoryNames)
      ? setRepositoryNames(searchParams.getAll(FilterValues.RepositoryNames))
      : setRepositoryNames([]);
    !!searchParams.get(FilterValues.DependsOnModels)
      ? setDependsOnModels(searchParams.getAll(FilterValues.DependsOnModels))
      : setDependsOnModels([]);
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
                {...register("searchInput")}
                InputProps={{
                  ...params.InputProps,
                  style: {
                    padding: 10,
                  },
                  startAdornment: <SearchIcon color="disabled" />,
                  endAdornment: (
                    <Tooltip title={t("reset-search")}>
                      <IconButton
                        sx={{ visibility: watch("searchInput") !== "" || models?.length > 0 ? "visible" : "hidden" }}
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
      {models !== null && <Results searchUrl={searchUrl} models={models} repositoryTree={repositoryTree}></Results>}
    </Box>
  );
}
