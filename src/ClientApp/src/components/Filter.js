import React, { useState, useEffect } from "react";
import {
  Autocomplete,
  Box,
  Button,
  Checkbox,
  FormGroup,
  FormControlLabel,
  Paper,
  Stack,
  Switch,
  TextField,
  Typography,
} from "@mui/material";
import { useForm, Controller } from "react-hook-form";
import { useTranslation } from "react-i18next";
import TreeView from "@mui/lab/TreeView";
import TreeItem from "@mui/lab/TreeItem";
import { SchemaLanguages } from "./SchemaLanguages";

export function Filter(props) {
  const { models, filteredModels, setFilteredModels, setPage, repositoryTree } = props;
  const [filterApplied, setFilterApplied] = useState(false);
  const [referencedModels, setReferencedModels] = useState([]);
  const [hideReferencedModelResults, setHideReferencedModelResults] = useState(false);
  const [allIssuerSelected, setAllIssuerSelected] = useState(false);
  const [allSchemaLanguageSelected, setAllSchemaLanguageSelected] = useState(true);

  const { t } = useTranslation("common");
  const { control, getValues, register, reset, setValue, handleSubmit, watch } = useForm();

  const onSubmit = (data) => {
    let filtered = models;
    if (Array.isArray(data.modelRepository)) {
      filtered = filtered.filter((m) => data.modelRepository.some((repo) => m.modelRepository.includes(repo)));
    }
    if (Array.isArray(data.issuer)) {
      filtered = filtered.filter((m) => data.issuer.includes(m.issuer));
    }
    if (Array.isArray(data.schemaLanguage)) {
      filtered = filtered.filter((m) => data.schemaLanguage.includes(m.schemaLanguage));
    }
    if (referencedModels.length > 0) {
      filtered = filtered.filter((m) => m.dependsOnModel.some((m) => referencedModels.includes(m)));
    }
    if (hideReferencedModelResults) {
      filtered = filtered.filter((m) => m.isDependOnModelResult === false);
    }
    setFilteredModels(filtered);
    setPage(1);
  };

  // Get and sort filter options
  const getIssuerWithoutPrefix = (issuer) => issuer.replace("https://", "").replace("http://", "").replace("www.", "");

  const schemaLanguageOptions = [...new Set(models.map((m) => m.schemaLanguage))].sort((a, b) => a.localeCompare(b));
  const issuerOptions = [...new Set(models.filter((m) => m.issuer !== null).map((m) => m.issuer))].sort((a, b) =>
    getIssuerWithoutPrefix(a).localeCompare(getIssuerWithoutPrefix(b))
  );
  const currentDependsOnModelOptions = [...new Set(filteredModels.flatMap((m) => m.dependsOnModel))].sort((a, b) =>
    a.localeCompare(b)
  );

  // Set default checkboxes checked for tree
  useEffect(() => {
    setChildrenCheckStatus(repositoryTree, true);
    checkAllSchemaLanguage(true);
    checkAllIssuer(true);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const updateIfAllChecked = (fieldName, optionsArray, setter) => {
    setter(allChecked(fieldName, optionsArray));
  };

  const allChecked = (fieldName, optionsArray) =>
    Object.entries(optionsArray).every(([k, v]) => getValues(fieldName + k) === true);

  const allSame = (fieldName, optionsArray) => {
    return Object.entries(optionsArray).every(([k, v]) => getValues(fieldName + k) === getValues(fieldName + 0));
  };

  const checkAllSchemaLanguage = (checked) => {
    setAllSchemaLanguageSelected(checked);
    Object.entries(schemaLanguageOptions).forEach(([k, v]) => {
      setValue("schemaLanguage" + k, checked);
    });
    checked ? setValue("schemaLanguage", null) : setValue("schemaLanguage", []);
  };

  const checkAllIssuer = (checked) => {
    setAllIssuerSelected(checked);
    Object.entries(issuerOptions).forEach(([k, v]) => {
      setValue("issuer" + k, checked);
    });
    checked ? setValue("issuer", null) : setValue("issuer", []);
  };

  const resetFilter = () => {
    reset();
    setFilteredModels(models);
    setFilterApplied(false);
    setReferencedModels([]);
    setHideReferencedModelResults(false);
    checkAllSchemaLanguage(true);
    checkAllIssuer(true);
    setChildrenCheckStatus(repositoryTree, true);
  };

  function getAllRepoNames(repositoryTree) {
    return [repositoryTree.name, ...repositoryTree.subsidiarySites.flatMap((r) => getAllRepoNames(r))];
  }

  function setChildrenCheckStatus(repositoryTree, checked) {
    const reposToCheck = getAllRepoNames(repositoryTree);
    reposToCheck.forEach((name) => setValue("modelRepository" + name, checked));
  }

  const renderTree = (repositoryTree) => (
    <TreeItem key={repositoryTree.name} nodeId={repositoryTree.name}>
      <FormGroup>
        <FormControlLabel
          control={
            <Controller
              name={"modelRepository" + repositoryTree.name}
              control={control}
              render={({ field }) => (
                <Checkbox
                  {...field}
                  // The additional register with the same name for all mapped checkboxes
                  // allows to pass all values of this type as an array to the filter.
                  {...register("modelRepository")}
                  checked={!!watch("modelRepository" + repositoryTree.name)}
                  onChange={(e) => {
                    field.onChange(e.target.checked);
                    setChildrenCheckStatus(repositoryTree, e.target.checked);
                  }}
                  value={repositoryTree.name}
                />
              )}
            />
          }
          label={repositoryTree.name}
        />
      </FormGroup>
      {Array.isArray(repositoryTree.subsidiarySites)
        ? repositoryTree.subsidiarySites.map((node) => renderTree(node))
        : null}
    </TreeItem>
  );

  return (
    <Paper sx={{ padding: 3, marginTop: 2, bgcolor: "action.hover" }}>
      <form onSubmit={handleSubmit(onSubmit)} name="search-form">
        <Stack direction="row" justifyContent="space-between" alignItems="flex-start" spacing={2}>
          <Box>
            <Typography variant="h6"> {t("model-repositories")}</Typography>
            <TreeView sx={{ marginLeft: -2 }} expanded={getAllRepoNames(repositoryTree)}>
              {renderTree(repositoryTree)}
            </TreeView>
          </Box>
          <Box>
            <Typography variant="h6">{t("issuer")}</Typography>
            <FormGroup>
              <FormControlLabel
                control={
                  <Checkbox
                    checked={allIssuerSelected}
                    indeterminate={!allSame("issuer", issuerOptions)}
                    onChange={(e) => {
                      checkAllIssuer(e.target.checked);
                    }}
                  />
                }
                label={t("all")}
              />
            </FormGroup>
            <Box sx={{ maxHeight: 900, overflowY: "auto", overflowX: "hidden" }}>
              {Object.entries(issuerOptions).map(([k, v]) => (
                <FormGroup sx={{ marginLeft: 2 }} key={k}>
                  <FormControlLabel
                    control={
                      <Controller
                        name={"issuer" + k}
                        control={control}
                        render={({ field }) => (
                          <Checkbox
                            {...field}
                            {...register("issuer")}
                            checked={!!watch("issuer" + k)}
                            onChange={(e) => {
                              field.onChange(e.target.checked);
                              updateIfAllChecked("issuer", issuerOptions, setAllIssuerSelected);
                            }}
                            value={v}
                          />
                        )}
                      />
                    }
                    label={v}
                  />
                </FormGroup>
              ))}
            </Box>
          </Box>
          <Box>
            {schemaLanguageOptions && schemaLanguageOptions?.length > 0 && (
              <React.Fragment>
                <Typography variant="h6"> {t("schema-language")}</Typography>
                <FormGroup>
                  <FormControlLabel
                    control={
                      <Checkbox
                        checked={allSchemaLanguageSelected}
                        indeterminate={!allSame("schemaLanguage", schemaLanguageOptions)}
                        onChange={(e) => {
                          checkAllSchemaLanguage(e.target.checked);
                        }}
                      />
                    }
                    label={t("all")}
                  />
                </FormGroup>
                {Object.entries(schemaLanguageOptions).map(([k, v]) => (
                  <FormGroup sx={{ marginLeft: 2 }} key={k}>
                    <FormControlLabel
                      control={
                        <Controller
                          name={"schemaLanguage" + k}
                          control={control}
                          render={({ field }) => (
                            <Checkbox
                              {...field}
                              {...register("schemaLanguage")}
                              checked={!!watch("schemaLanguage" + k)}
                              onChange={(e) => {
                                field.onChange(e.target.checked);
                                updateIfAllChecked(
                                  "schemaLanguage",
                                  schemaLanguageOptions,
                                  setAllSchemaLanguageSelected
                                );
                              }}
                              value={v}
                            />
                          )}
                        />
                      }
                      label={SchemaLanguages[v]}
                    />
                  </FormGroup>
                ))}
              </React.Fragment>
            )}
          </Box>
        </Stack>
        <Box>
          {models.some((m) => m.isDependOnModelResult === true) && (
            <FormGroup>
              <FormControlLabel
                control={
                  <Switch
                    checked={hideReferencedModelResults}
                    onChange={(e) => setHideReferencedModelResults(e.target.checked)}
                  />
                }
                label={t("hide-results-for-depends-on-models")}
              />
            </FormGroup>
          )}
          <Typography mt={8} variant="h6">
            {t("referenced-models")}
          </Typography>
          <Autocomplete
            multiple
            options={currentDependsOnModelOptions}
            noOptionsText={t("no-depends-on-model")}
            value={referencedModels}
            onChange={(e, data) => {
              setReferencedModels(data);
            }}
            renderInput={(params) => (
              <TextField {...params} type="text" variant="standard" placeholder={t("model-name")} />
            )}
          />
        </Box>
        <Stack mt={5} direction="row" alignItems="flex-end" justifyContent="flex-end">
          {filterApplied && (
            <Button type="reset" onClick={resetFilter} variant="text">
              {t("reset")}
            </Button>
          )}
          <Button type="submit" onClick={() => setFilterApplied(true)} variant="outlined">
            {t("apply-filter")}
          </Button>
        </Stack>
      </form>
    </Paper>
  );
}
