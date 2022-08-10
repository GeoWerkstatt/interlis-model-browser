import React, { useEffect, useState } from "react";
import { AppBar, Container, Toolbar, Typography } from "@mui/material";

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
      <AppBar position="static">
        <Toolbar>
          <Typography>INTERLIS Model Repo Browser</Typography>
        </Toolbar>
      </AppBar>
      <Container>{props.children}</Container>
      <footer>
        <Typography variant="body2" gutterBottom>
          Version: {version}
        </Typography>
      </footer>
    </div>
  );
}
