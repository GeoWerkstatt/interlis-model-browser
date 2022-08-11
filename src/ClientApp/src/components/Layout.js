import React, { useEffect, useState } from "react";
import { AppBar, Container, Toolbar, Typography } from "@mui/material";
import { Box } from "@mui/system";

export function Layout(props) {
  const [version, setVersion] = useState();
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
            <Typography variant="caption" gutterBottom>
              Version: {version}
            </Typography>
          </Toolbar>
        </AppBar>
      </Box>
      <Container>{props.children}</Container>
    </div>
  );
}
