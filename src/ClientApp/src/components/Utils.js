export const getAllModels = (repositoryNode) => {
  const modelRepository = repositoryNode.title + " [" + repositoryNode.name + "]";

  return [
    ...repositoryNode.models?.map((m) => {
      m.modelRepository = modelRepository;
      return m;
    }),
    ...repositoryNode.subsidiarySites.flatMap((r) => getAllModels(r)),
  ];
};

export const sortRepositoryTree = (repositoryNode) => {
  repositoryNode.sort((a, b) => a.name.localeCompare(b.name));
  repositoryNode.forEach((node) => {
    if (node.subsidiarySites.length > 0) {
      sortRepositoryTree(node.subsidiarySites);
    }
  });
};

export const filterRepoTree = (repositoryNode, repositoryNames) => {
  if (repositoryNode) {
    if (repositoryNode.subsidiarySites.length > 0) {
      repositoryNode.subsidiarySites.forEach((node) => {
        filterRepoTree(node);
      });
    }
    if (repositoryNames?.length > 0 && !repositoryNames.includes(repositoryNode.name)) {
      repositoryNode.models = [];
    }
  }
};
