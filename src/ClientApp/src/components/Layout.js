import React, { useEffect, useState } from "react";
import CodeIcon from "@mui/icons-material/Code";
import { AppBar, Box, Container, Toolbar, Tooltip, Typography } from "@mui/material";
import { EmbedDialog } from "./EmbedDialog";
import { useTranslation } from "react-i18next";

export function Layout(props) {
  const [version, setVersion] = useState();
  const [open, setOpen] = useState(false);
  const { t } = useTranslation("common");

  const handleClickOpen = () => {
    setOpen(true);
  };

  useEffect(() => {
    async function fetchData() {
      const response = await fetch("/version");
      setVersion(await response.text());
    }

    fetchData();
  }, []);

  return (
    <div>
      <Box sx={{ flexGrow: 1 }}>
        <AppBar position="static">
          <Toolbar>
            <Typography sx={{ flexGrow: 1 }}>INTERLIS Model Repo Browser</Typography>
            <Tooltip title={t("generate-embed-tag")}>
              <CodeIcon sx={{ marginBottom: 0.5, marginRight: 1 }} onClick={handleClickOpen}></CodeIcon>
            </Tooltip>
            <Typography variant="caption" gutterBottom>
              Version: {version}
            </Typography>
          </Toolbar>
        </AppBar>
      </Box>
      <Container>{props.children}</Container>
      <EmbedDialog open={open} setOpen={setOpen}></EmbedDialog>
    </div>
  );
}
