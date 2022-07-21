import React, { useEffect, useState } from "react";
import Container from "@mui/material/Container";
import Typography from "@mui/material/Typography";

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
      <Container>{props.children}</Container>
      <footer>
        <Typography variant="body2" gutterBottom>
          Version: {version}
        </Typography>
      </footer>
    </div>
  );
}
