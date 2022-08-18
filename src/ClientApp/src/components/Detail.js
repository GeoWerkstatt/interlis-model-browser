import React, { useEffect, useState } from "react";
import { useNavigate, useLocation, useParams } from "react-router-dom";
import { Box, Button, Chip, Stack, TextField, Typography } from "@mui/material";
import CloudQueueIcon from "@mui/icons-material/CloudQueue";
import SellIcon from "@mui/icons-material/Sell";
import EditIcon from "@mui/icons-material/Edit";
import InfoIcon from "@mui/icons-material/Info";
import MailIcon from "@mui/icons-material/Mail";
import RestoreIcon from "@mui/icons-material/Restore";
import FlagIcon from "@mui/icons-material/Flag";
import HubIcon from "@mui/icons-material/Hub";
import InsertDriveFileIcon from "@mui/icons-material/InsertDriveFile";
import ArrowBackIosIcon from "@mui/icons-material/ArrowBackIos";
import { useTranslation } from "react-i18next";

export function Detail() {
  const [model, setModel] = useState();
  const [modelText, setModelText] = useState("");
  const { t } = useTranslation("common");

  const location = useLocation();
  const navigate = useNavigate();
  const { md5, name } = useParams();

  const backToSearch = () => navigate(`/?query=${location.state.query}`, { replace: true });
  const toHome = () => navigate("/");

  async function getModel(md5, name) {
    const response = await fetch("/model/" + md5 + "/" + name);
    if (response.ok) {
      if (response.status === 204 /* No Content */) {
        setModel();
      } else {
        const model = await response.json();
        setModel(model);
      }
    } else {
      setModel();
    }
  }

  async function getModelPreview(model) {
    const response = await fetch(model.uri);
    setModelText(await response.text());
  }

  useEffect(() => {
    getModel(md5, name);
    getModelPreview(model);
  }, [md5, model, name]);

  return (
    <Box mt={10}>
      {location?.state !== null ? (
        <Button variant="outlined" onClick={backToSearch} startIcon={<ArrowBackIosIcon />}>
          {t("back-to-search")}
        </Button>
      ) : (
        <Button variant="outlined" onClick={toHome} startIcon={<ArrowBackIosIcon />}>
          {t("to-search")}
        </Button>
      )}
      {model && (
        <>
          <Stack direction="row" alignItems="flex-end" flexWrap="wrap" sx={{ color: "text.secondary" }}>
            <Typography mt={5} variant="h4">
              {model?.name}
            </Typography>
            <Box>{model.tags && model.tags.map((tag) => <Chip key={tag} sx={{ margin: 1 }} label={tag} />)}</Box>
          </Stack>
          <Stack direction="column" alignItems="flex-start" sx={{ color: "text.secondary" }}>
            <Box ml={1} mt={1}>
              <SellIcon sx={{ marginBottom: -0.5, marginRight: 0.5 }} />
              {t("schema-language")}: {model.schemaLanguage.substring(3)}
            </Box>
            <Box ml={1} mt={1}>
              <CloudQueueIcon sx={{ marginBottom: -0.5, marginRight: 0.5 }} />
              {t("model-repository")}: {model.modelRepository.title + " [" + model.modelRepository.name + "]"}
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
            <Box ml={1} mt={1}>
              <HubIcon sx={{ marginBottom: -0.5, marginRight: 0.5 }} />
              {t("referenced-models")}:
              {model.dependsOnModel &&
                model.dependsOnModel.map((m) => (
                  <Chip sx={{ marginLeft: 1, marginBottom: 1 }} key={m} label={m} variant="outlined">
                    {m}
                  </Chip>
                ))}
            </Box>
            {model.technicalContact && (
              <Box ml={1} mt={1}>
                <MailIcon sx={{ marginBottom: -0.5, marginRight: 0.5 }} />
                <a href={model.technicalContact} target="_blank" rel="noreferrer">
                  {t("technical-contact")}
                </a>
              </Box>
            )}
            {model.furtherInformation && (
              <Box ml={1} mt={1}>
                <InfoIcon sx={{ marginBottom: -0.5, marginRight: 0.5 }} />
                <a href={model.furtherInformation} target="_blank" rel="noreferrer">
                  {t("more-information")}
                </a>
              </Box>
            )}

            <TextField
              sx={{ bgcolor: "action.hover", marginTop: 5 }}
              variant="outlined"
              multiline
              rows={15}
              fullWidth
              label={t("model-preview")}
              inputProps={{ style: { fontSize: 12 } }}
              InputLabelProps={{ style: { fontSize: 22 } }}
              InputProps={{ readOnly: true, style: { fontSize: 22 } }}
              value={modelText}
              focused={false}
            />
          </Stack>
        </>
      )}
      {!model && (
        <Typography mt={5} variant="h6">
          {t("invalid-model-url")}
        </Typography>
      )}
    </Box>
  );
}
