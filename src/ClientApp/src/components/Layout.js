import React, { useEffect, useState } from "react";
import CodeIcon from "@mui/icons-material/Code";
import { AppBar, Box, Container, FormControl, MenuItem, Select, Toolbar, Tooltip, Typography } from "@mui/material";
import { EmbedDialog } from "./EmbedDialog";
import { useTranslation } from "react-i18next";

export function Layout(props) {
  const [version, setVersion] = useState();
  const [open, setOpen] = useState(false);
  const { t, i18n } = useTranslation("common");

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
            <Typography sx={{ flexGrow: 1 }}>INTERLIS Model Browser</Typography>
            <Tooltip title={t("generate-embed-tag")}>
              <CodeIcon sx={{ marginBottom: 0.5, marginRight: 1 }} onClick={handleClickOpen}></CodeIcon>
            </Tooltip>
            <FormControl variant="standard">
              <Select
                disableUnderline
                sx={{ ml: 3, color: "white", backgroundColor: "none" }}
                value={i18n.language}
                onChange={(e) => i18n.changeLanguage(e.target.value)}
                inputProps={{
                  styles: { color: "white" },
                }}
              >
                <MenuItem value={"de"}>DE</MenuItem>
                <MenuItem value={"fr"}>FR</MenuItem>
                <MenuItem value={"it"}>IT</MenuItem>
              </Select>
            </FormControl>
          </Toolbar>
        </AppBar>
      </Box>
      <Container>{props.children}</Container>
      <EmbedDialog open={open} setOpen={setOpen}></EmbedDialog>
      <Typography sx={{ position: "fixed", bottom: 1, right: 1, color: "darkgrey" }} variant="caption" gutterBottom>
        Version: {version}
      </Typography>
    </div>
  );
}
