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
  TextField,
  Typography,
} from "@mui/material";
import { useForm, Controller } from "react-hook-form";
import { useTranslation } from "react-i18next";
import TreeView from "@mui/lab/TreeView";
import TreeItem from "@mui/lab/TreeItem";

export function Filter(props) {
  const { models, filteredModels, setFilteredModels, repositoryTree } = props;
  const [filterApplied, setFilterApplied] = useState(false);
  const [referencedModels, setReferencedModels] = useState([]);
  const [issuerFilterSelected, setIssuerFilterSelected] = useState(false);
  const [schemaLanguageFilterSelected, setSchemaLanguageFilterSelected] = useState(false);

  const { t } = useTranslation("common");
  const { control, register, reset, resetField, setValue, handleSubmit, watch } = useForm();

  const onSubmit = (data) => {
    let filtered = models;
    if (Array.isArray(data.modelRepository)) {
      filtered = filtered.filter((m) => data.modelRepository.some((repo) => m.modelRepository.includes(repo)));
    }
    if (data.issuer?.length > 0) {
      filtered = filtered.filter((m) => data.issuer.includes(m.issuer));
    }
    if (data.schemaLanguage?.length > 0) {
      filtered = filtered.filter((m) => data.schemaLanguage.includes(m.schemaLanguage));
    }
    if (referencedModels.length > 0) {
      filtered = filtered.filter((m) => m.dependsOnModel.some((m) => referencedModels.includes(m)));
    }
    setFilteredModels(filtered);
  };

  const schemaLanguageOptions = [...new Set(models.map((m) => m.schemaLanguage))];
  const issuerOptions = [...new Set(models.filter((m) => m.issuer !== null).map((m) => m.issuer))];

  const currentDependsOnModelOptions = [...new Set(filteredModels.flatMap((m) => m.dependsOnModel))];

  // Set default checkboxes checked for tree
  useEffect(() => {
    setChildrenCheckStatus(repositoryTree, true);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const resetIssuerFilter = () => {
    Object.entries(issuerOptions).forEach(([k, v]) => {
      resetField("issuer" + k);
      resetField("issuer");
    });
  };

  const resetSchemaLangugageFilter = () => {
    Object.entries(schemaLanguageOptions).forEach(([k, v]) => {
      resetField("schemaLanguage" + k);
      resetField("schemaLanguage");
    });
  };

  const resetFilter = () => {
    setIssuerFilterSelected(false);
    setSchemaLanguageFilterSelected(false);
    setFilteredModels(models);
    setFilterApplied(false);
    setReferencedModels([]);
    reset();
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
            <TreeView expanded={getAllRepoNames(repositoryTree)}>{renderTree(repositoryTree)}</TreeView>
          </Box>
          <Box>
            <Typography variant="h6">{t("issuer")}</Typography>
            <FormGroup>
              <FormControlLabel
                control={
                  <Checkbox
                    checked={!issuerFilterSelected}
                    onChange={(e) => {
                      resetIssuerFilter();
                      setIssuerFilterSelected(false);
                    }}
                  />
                }
                label={t("all")}
              />
            </FormGroup>
            {Object.entries(issuerOptions).map(([k, v]) => (
              <FormGroup>
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
                            setIssuerFilterSelected(true);
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
          <Box>
            {schemaLanguageOptions && schemaLanguageOptions?.length > 1 && (
              <React.Fragment>
                <Typography variant="h6"> {t("schema-language")}</Typography>
                <FormGroup>
                  <FormControlLabel
                    control={
                      <Checkbox
                        checked={!schemaLanguageFilterSelected}
                        onChange={(e) => {
                          resetSchemaLangugageFilter();
                          setSchemaLanguageFilterSelected(false);
                        }}
                      />
                    }
                    label={t("all")}
                  />
                </FormGroup>
                {Object.entries(schemaLanguageOptions).map(([k, v]) => (
                  <FormGroup>
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
                                setSchemaLanguageFilterSelected(true);
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
              </React.Fragment>
            )}
          </Box>
        </Stack>
        <Box>
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
