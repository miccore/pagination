# GitHub Actions - Guide de Configuration

Ce document décrit la configuration des workflows CI/CD pour publier automatiquement le package NuGet.

## 📁 Structure des workflows

```
.github/workflows/
├── ci.yml              # Build et tests sur push/PR
├── release.yml         # Release automatique sur tag
└── publish-manual.yml  # Publication manuelle
```

---

## ⚙️ Configuration requise sur GitHub

### 1. Ajouter le secret `NUGET_API_KEY`

1. Aller sur https://www.nuget.org/account/apikeys
2. Créer une nouvelle clé API :
   - **Key Name:** `Miccore.Net.Pagination GitHub Actions`
   - **Expiration:** 365 days (ou selon votre préférence)
   - **Scope:** Push new packages and package versions
   - **Glob Pattern:** `Miccore.Net.Pagination`
3. Copier la clé générée
4. Dans votre repository GitHub :
   - Aller à **Settings** → **Secrets and variables** → **Actions**
   - Cliquer **New repository secret**
   - Name: `NUGET_API_KEY`
   - Value: Coller la clé NuGet

### 2. (Optionnel) Créer l'environnement `nuget-publish`

Pour ajouter une approbation manuelle avant chaque publication :

1. GitHub → **Settings** → **Environments**
2. Cliquer **New environment**
3. Name: `nuget-publish`
4. Cocher **Required reviewers** et ajouter les approbateurs
5. Sauvegarder

### 3. (Optionnel) Configurer Codecov

Pour les rapports de couverture de code :

1. Aller sur https://codecov.io et connecter votre repository
2. Copier le token
3. Ajouter le secret `CODECOV_TOKEN` dans GitHub

---

## 🚀 Utilisation

### Méthode 1 : Release automatique avec tag

```bash
# Créer et pousser un tag de version
git tag v2.0.0
git push origin v2.0.0
```

Le workflow va automatiquement :
1. ✅ Builder le projet
2. ✅ Exécuter les tests
3. ✅ Créer le package NuGet
4. ✅ Publier sur NuGet.org
5. ✅ Créer une GitHub Release avec les assets

### Méthode 2 : Release manuelle

1. Aller sur GitHub → **Actions** → **Publish to NuGet (Manual)**
2. Cliquer **Run workflow**
3. Entrer la version (ex: `2.0.0` ou `2.1.0-beta.1`)
4. Cocher "Is this a pre-release?" si nécessaire
5. Cliquer **Run workflow**

---

## 📋 Formats de version supportés

| Format | Type | Exemple |
|--------|------|---------|
| `X.Y.Z` | Release stable | `2.0.0`, `2.1.0` |
| `X.Y.Z-alpha.N` | Alpha | `2.1.0-alpha.1` |
| `X.Y.Z-beta.N` | Beta | `2.1.0-beta.2` |
| `X.Y.Z-rc.N` | Release Candidate | `2.1.0-rc.1` |

---

## 📊 Résumé des workflows

| Workflow | Déclencheur | Actions |
|----------|-------------|---------|
| **CI** | Push/PR sur `main`, `develop`, `feature/**` | Build + Tests (.NET 8 & 10) |
| **Release** | Push tag `v*.*.*` | Build + Tests + NuGet + GitHub Release |
| **Manual** | Workflow dispatch | Build + Tests + NuGet + Tag + Release |

---

## 🔗 Liens utiles

- [NuGet API Keys](https://www.nuget.org/account/apikeys)
- [GitHub Encrypted Secrets](https://docs.github.com/en/actions/security-guides/encrypted-secrets)
- [GitHub Environments](https://docs.github.com/en/actions/deployment/targeting-different-environments/using-environments-for-deployment)
