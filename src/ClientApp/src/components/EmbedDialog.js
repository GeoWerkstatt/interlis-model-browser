import React, { useState } from "react";
import {
  Button,
  Dialog,
  DialogTitle,
  DialogActions,
  DialogContent,
  DialogContentText,
  IconButton,
  Stack,
  Switch,
  TextField,
  Tooltip,
} from "@mui/material";
import FormGroup from "@mui/material/FormGroup";
import FormControlLabel from "@mui/material/FormControlLabel";
import ContentCopyIcon from "@mui/icons-material/ContentCopy";
import { useTranslation } from "react-i18next";
import InputAdornment from "@mui/material/InputAdornment";

export function EmbedDialog(props) {
  const { open, setOpen } = props;
  const [width, setWidth] = useState(900);
  const [height, setHeight] = useState(750);
  const [border, setBorder] = useState(true);
  const { t } = useTranslation("common");

  const handleClose = () => {
    setOpen(false);
  };

  return (
    <Dialog open={open} onClose={handleClose} fullWidth={true} maxWidth="md">
      <DialogTitle>{t("generate-embed-tag")}</DialogTitle>
      <DialogContent>
        <DialogContentText>{t("generate-embed-tag-instructions")}</DialogContentText>
        <Stack mt={5} direction="row" justifyContent="flex-start" alignItems="flex-end">
          <TextField
            margin="dense"
            onChange={(e) => setWidth(e.target.value)}
            InputProps={{
              endAdornment: <InputAdornment position="start">px</InputAdornment>,
            }}
            value={width}
            label={t("width")}
            sx={{ marginRight: 5 }}
            type="number"
            variant="standard"
          />
          <TextField
            margin="dense"
            onChange={(e) => setHeight(e.target.value)}
            InputProps={{
              endAdornment: <InputAdornment position="start">px</InputAdornment>,
            }}
            value={height}
            label={t("height")}
            sx={{ marginRight: 5 }}
            type="number"
            variant="standard"
          />
          <FormGroup>
            <FormControlLabel
              control={<Switch checked={border} onChange={(e) => setBorder(e.target.checked)} />}
              label={t("border")}
            />
          </FormGroup>
        </Stack>
        <Stack mt={5} direction="row" alignItems="flex-end">
          <TextField
            value={`<iframe src="https://ilimodels.ch" width="${width}" height="${height}" style="border: ${
              border ? "1px solid darkgrey" : "none"
            } " sandbox="allow-scripts allow-same-origin allow-storage-access-by-user-activation allow-forms"></iframe>`}
            sx={{ bgcolor: "action.hover", marginTop: 5 }}
            type="text"
            fullWidth
            variant="outlined"
          />
          <Tooltip title={t("copy-to-clipboard")}>
            <IconButton
              onClick={() => {
                navigator.clipboard.writeText(
                  `<iframe src="https://ilimodels.ch" width="${width}" height="${height}" style="border: ${
                    border ? "1px solid darkgrey" : "none"
                  } " sandbox="allow-scripts allow-same-origin allow-storage-access-by-user-activation allow-forms"></iframe>`
                );
              }}
            >
              <ContentCopyIcon />
            </IconButton>
          </Tooltip>
        </Stack>
      </DialogContent>
      <DialogActions>
        <Button onClick={handleClose}>{t("close")}</Button>
      </DialogActions>
    </Dialog>
  );
}
