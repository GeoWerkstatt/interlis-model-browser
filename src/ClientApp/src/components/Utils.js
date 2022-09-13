export function getAllModels(repository) {
  const modelRepository = repository.title + " [" + repository.name + "]";

  return [
    ...repository.models.map((m) => {
      m.modelRepository = modelRepository;
      return m;
    }),
    ...repository.subsidiarySites.flatMap((r) => getAllModels(r)),
  ];
}
