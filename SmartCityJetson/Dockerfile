# ============================================
# 1) Baza L4T/JetPack cu CUDA, TensorRT, cuDNN si OpenCV (cu suport CUDA)
# ============================================
FROM nvcr.io/nvidia/l4t-jetpack:r36.4.0

# ============================================
# 2) Argument si grup gpio pentru permisiuni
# ============================================
ARG GPIO_GID=997
RUN groupadd -g ${GPIO_GID} gpio

# ============================================
# 3) Setarri de mediu pentru Conda si pip
# ============================================
ENV PATH="/opt/conda/bin:${PATH}"
#ENV PIP_INDEX_URL="https://pypi.jetson-ai-lab.dev/jp6/cu126"

# ============================================
# 4) Instalare pachete de baza+ udev + build-essential
# ============================================
RUN apt-get update && apt-get install -y \
      udev \
      wget \
      ca-certificates \
      build-essential \
      python3-dev \
      openssl \
      python3-pip \
      libtbb2 \
      libtbb-dev \
      #i2c-tools \
      #python3-smbus \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

# ============================================
# 5) Meson necesar pentru GLib si update certificates
# ============================================
RUN pip install meson==1.4.0 \
 && update-ca-certificates

# ============================================
# 6) Adaugare biblioteci HDF5 din host + ldconfig
# ============================================
COPY hdf5_libs/* /usr/lib/aarch64-linux-gnu/
RUN ldconfig && ldconfig -p

# ============================================
# 7) Atlas BLAS (libatlas3-base + dev)
# ============================================
RUN wget https://ftp.debian.org/debian/pool/main/a/atlas/libatlas3-base_3.10.3-13_arm64.deb -O /tmp/libatlas3-base.deb \
 && dpkg -i /tmp/libatlas3-base.deb \
 && rm /tmp/libatlas3-base.deb \
 && wget https://ftp.debian.org/debian/pool/main/a/atlas/libatlas-base-dev_3.10.3-13_arm64.deb -O /tmp/libatlas-base-dev.deb \
 && dpkg -i /tmp/libatlas-base-dev.deb \
 && rm /tmp/libatlas-base-dev.deb

# ============================================
# 8) Descarcare si verificare instalare Miniforge (Conda)
# ============================================
RUN wget --no-check-certificate \
      https://github.com/conda-forge/miniforge/releases/latest/download/Miniforge3-Linux-aarch64.sh \
      -O /miniforge.sh \
 && chmod +x /miniforge.sh \
 && /bin/bash /miniforge.sh -b -p /opt/conda \
 && rm /miniforge.sh \
 && /opt/conda/bin/conda clean --all --yes

# ============================================
# 9) Activare Conda in shell-uri interactive
# ============================================
RUN ln -s /opt/conda/etc/profile.d/conda.sh /etc/profile.d/conda.sh \
 && echo ". /opt/conda/etc/profile.d/conda.sh" >> ~/.bashrc \
 && echo "source /opt/conda/etc/profile.d/conda.sh && conda activate SmartCityJetson_env" >> ~/.bashrc

# ============================================
# 10) Variabile runtime CUDA/Python
# ============================================
ENV LD_LIBRARY_PATH="/opt/conda/envs/SmartCityJetson_env/lib:/usr/lib/aarch64-linux-gnu"
ENV LD_PRELOAD="/usr/lib/aarch64-linux-gnu/libtinfo.so.6"
ENV PYTHONPATH="/opt/conda/envs/SmartCityJetson_env/lib/python3.10/site-packages:/usr/lib/python3.10/dist-packages"


# ============================================
# 11) Copiem wheel-urile CUDA in container
# ============================================
COPY cuda_wheels/ /tmp/cuda_wheels/

# ============================================
# 12) Creare mediu Conda si instalarea pachetelor
# ============================================
WORKDIR /app
COPY environment.yml /app/
RUN /opt/conda/bin/conda env create -f environment.yml

ENV CONDA_DEFAULT_ENV=SmartCityJetson_env
ENV PATH=/opt/conda/envs/SmartCityJetson_env/bin:/opt/conda/bin:$PATH
ENV PYTHONNOUSERSITE=1
# ============================================
# 13) FORCE-reinstall GPU wheels *and* ignore any index
# ============================================
RUN /opt/conda/bin/conda run -n SmartCityJetson_env \
      pip install --no-index --no-deps --force-reinstall \
        /tmp/cuda_wheels/torch-2.6.0-cp310-cp310-linux_aarch64.whl \
        /tmp/cuda_wheels/torchvision-0.21.0-cp310-cp310-linux_aarch64.whl \
        /tmp/cuda_wheels/torchaudio-2.6.0-cp310-cp310-linux_aarch64.whl \
        /tmp/cuda_wheels/onnxruntime_gpu-1.20.2-cp310-cp310-linux_aarch64.whl \
        /tmp/cuda_wheels/opencv_contrib_python-4.10.0.86-cp310-cp310-linux_aarch64.whl \
        /tmp/cuda_wheels/opencv_python-4.10.0-py3-none-any.whl \
    && rm -rf /tmp/cuda_wheels


# ============================================
# 14) Instalare Jetson.GPIO si permisiuni GPIO
# ============================================
RUN /opt/conda/envs/SmartCityJetson_env/bin/pip install --force-reinstall Jetson.GPIO \
 && cp /opt/conda/envs/SmartCityJetson_env/lib/python3.10/site-packages/Jetson/GPIO/99-gpio.rules \
       /etc/udev/rules.d/99-gpio.rules \
 && usermod -aG gpio root

# ============================================
# 14) Copiere cod sursa si entrypoint pentru alinierea GID
# ============================================
COPY . /app
COPY entrypoint.sh /entrypoint.sh
RUN chmod +x /entrypoint.sh
ENTRYPOINT ["/entrypoint.sh"]

# ============================================
# 15) Shell cu Conda activat si CMD implicit
# ============================================
SHELL ["/bin/bash", "-c", "source /opt/conda/etc/profile.d/conda.sh && conda activate SmartCityJetson_env && bash"]
#CMD ["conda", "run", "-n", "SmartCityJetson_env", "python", "ServerCam.py"]
CMD ["/opt/conda/envs/SmartCityJetson_env/bin/python", "ServerCam.py"]
