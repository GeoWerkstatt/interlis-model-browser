import React, { useState, useEffect } from "react";
import { Link } from "react-router-dom";
import { Box, Button, Chip, Pagination, Stack, Typography } from "@mui/material";
import CloudQueueIcon from "@mui/icons-material/CloudQueue";
import SellIcon from "@mui/icons-material/Sell";
import EditIcon from "@mui/icons-material/Edit";
import InfoIcon from "@mui/icons-material/Info";
import RestoreIcon from "@mui/icons-material/Restore";
import FilterAltIcon from "@mui/icons-material/FilterAlt";
import FlagIcon from "@mui/icons-material/Flag";
import InsertDriveFileIcon from "@mui/icons-material/InsertDriveFile";
import { useTranslation } from "react-i18next";
import { Filter } from "./Filter";

export function Results(props) {
  const { models, repositoryTree, searchUrl } = props;
  const hideFilter = searchUrl.searchParams.get("hideFilter") === "true";
  const { t } = useTranslation("common");
  const [page, setPage] = useState(1);
  const [showFilter, setShowFilter] = useState(false);
  const [filteredModels, setFilteredModels] = useState(models);

  const modelsPerPage = 10;

  const handleChangePage = (event, newPage) => {
    setPage(newPage);
  };

  const toggleFilter = () => {
    setShowFilter(!showFilter);
    setFilteredModels(models);
  };

  useEffect(() => {
    setFilteredModels(models);
    setShowFilter(false);
  }, [models]);

  return (
    <React.Fragment>
      <Stack direction="row" justifyContent="space-between" alignItems="flex-end" spacing={2}>
        <Typography variant="h4" mt={6} ml={1}>
          {filteredModels.length + " " + t("models-found", { count: filteredModels.length })}
        </Typography>
        {!hideFilter && (
          <Button variant="outlined" startIcon={<FilterAltIcon />} onClick={toggleFilter}>
            {t("filter")}
          </Button>
        )}
      </Stack>
      {showFilter && !hideFilter && (
        <Filter
          models={models}
          filteredModels={filteredModels}
          repositoryTree={repositoryTree}
          setFilteredModels={setFilteredModels}
        ></Filter>
      )}
      <Box sx={{ width: "100%", bgcolor: "background.paper", marginBottom: 8 }}>
        {filteredModels &&
          filteredModels
            .sort((a, b) => new Date(b.publishingDate) - new Date(a.publishingDate))
            .slice((page - 1) * modelsPerPage, (page - 1) * modelsPerPage + modelsPerPage)
            .map((model) => (
              <Box key={model.id} mt={6} direction="column" alignItems="flex-start">
                <Stack direction="row" flexWrap="wrap" alignItems="center">
                  <Button sx={{ color: "text.primary" }}>
                    <Link
                      style={{ color: "inherit", textDecoration: "inherit", fontSize: 20, margin: -1 }}
                      to={{ pathname: "/detail/" + model.mD5 + "/" + model.name }}
                      state={{ searchQuery: searchUrl.search }}
                    >
                      {model.name}
                    </Link>
                  </Button>
                  {model.tags &&
                    model.tags.map((tag) => tag.length > 0 && <Chip key={tag} sx={{ margin: 1 }} label={tag} />)}
                </Stack>
                <Stack
                  direction={{ xs: "column", sm: "column", md: "row" }}
                  alignItems={{ xs: "flex-start", sm: "flex-start", md: "flex-end" }}
                  flexWrap="wrap"
                  sx={{ color: "text.secondary" }}
                >
                  <Box ml={1} mt={1}>
                    <SellIcon sx={{ marginBottom: -0.5, marginRight: 0.5 }} />
                    {t("schema-language")}: {model.schemaLanguage.substring(3)}
                  </Box>
                  <Box ml={1} mt={1}>
                    <CloudQueueIcon sx={{ marginBottom: -0.5, marginRight: 0.5 }} />
                    {t("model-repository")}: {model.modelRepository}
                  </Box>
                  <Box ml={1} mt={1}>
                    <FlagIcon sx={{ marginBottom: -0.5, marginRight: 0.5 }} />
                    {t("latest-version")}: {model.version}
                  </Box>
                  <Box ml={1} mt={1}>
                    <EditIcon sx={{ marginBottom: -0.5, marginRight: 0.5 }} />
                    {t("issuer")}: {model.issuer}
                  </Box>
                  <Box ml={1} mt={1}>
                    <RestoreIcon sx={{ marginBottom: -0.5, marginRight: 0.5 }} />
                    {t("last-updated")}: {new Date(model.publishingDate).toLocaleDateString()}
                  </Box>
                  <Box ml={1} mt={1}>
                    <InsertDriveFileIcon sx={{ marginBottom: -0.5, marginRight: 0.5 }} />
                    {t("file")}: {model.file}
                  </Box>
                  {model.furtherInformation && (
                    <Box ml={1} mt={1}>
                      <InfoIcon sx={{ marginBottom: -0.5, marginRight: 0.5 }} />
                      <a href={model.furtherInformation} target="_blank" rel="noreferrer">
                        {t("more-information")}
                      </a>
                    </Box>
                  )}
                </Stack>
              </Box>
            ))}
        {filteredModels && filteredModels.length > modelsPerPage && (
          <Pagination
            sx={{ marginTop: 3 }}
            page={page}
            count={Math.ceil(filteredModels.length / modelsPerPage)}
            onChange={handleChangePage}
          />
        )}
      </Box>
    </React.Fragment>
  );
}
