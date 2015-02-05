# -*- mode: ruby -*-
# vi: set ft=ruby :

Vagrant.configure(2) do |config|
  config.vm.box = "ubuntu/trusty64"
  config.vm.provision :shell, path: "up/mysql.sh"
  config.vm.provision :shell, path: "up/python.sh"
  config.vm.network :public_network, ip: "192.168.1.107"
end
