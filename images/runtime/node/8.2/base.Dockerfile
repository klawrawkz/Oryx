# The official Node 8.2 image has vulnerabilities, so we build our own version
# to fetch the latest strech release with the required fixes.
# https://github.com/nodejs/docker-node.git, commit ID 9b863beddf7dee9b8a6eba0dd8334b1d7980f958.
FROM oryx-node-run-base

RUN groupadd --gid 1000 node \
  && useradd --uid 1000 --gid node --shell /bin/bash --create-home node

RUN ARCH= && dpkgArch="$(dpkg --print-architecture)" \
  && case "${dpkgArch##*-}" in \
    amd64) ARCH='x64';; \
    ppc64el) ARCH='ppc64le';; \
    s390x) ARCH='s390x';; \
    *) echo "unsupported architecture"; exit 1 ;; \
  esac

ENV NPM_CONFIG_LOGLEVEL info
ENV NODE_VERSION 8.2.1

ARG IMAGES_DIR=/tmp/oryx/images
RUN ${IMAGES_DIR}/installPlatform.sh nodejs $NODE_VERSION --dir /usr/local --links false \
    && ln -s /usr/local/bin/node /usr/local/bin/nodejs
RUN ${IMAGES_DIR}/runtime/node/installDependencies.sh
RUN rm -rf /tmp/oryx

CMD [ "node" ]

